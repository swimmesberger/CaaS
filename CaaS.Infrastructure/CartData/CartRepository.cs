using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.CartAggregate;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.ProductAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Ado.Query.Parameters.Where;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.CartData; 

public class CartRepository : CrudReadRepository<CartDataModel, Cart>, ICartRepository {
    private new CartDomainModelConvert Converter => (CartDomainModelConvert)base.Converter;
    
    public CartRepository(IDao<CartDataModel> dao, IDao<ProductCartDataModel> cartItemDao, 
            IProductRepository productRepository, ICustomerRepository customerRepository, ICouponRepository couponRepository, ISystemClock timeProvider) : 
            base(dao, new CartDomainModelConvert(cartItemDao, productRepository, customerRepository, timeProvider, couponRepository)) {
    }

    public Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default) {
        return AnyAsync(new StatementParameters() {
            Where = new QueryParameter[]{new(nameof(CartDataModel.Id), id)}
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<Cart>> FindExpiredCartsAsync(int lifeTimeMinutes, CancellationToken cancellationToken = default) {
        var parameters = new List<QueryParameter> {
            new(nameof(Cart.LastAccess), WhereComparator.Less, Converter.TimeProvider.UtcNow.Subtract(TimeSpan.FromMinutes(lifeTimeMinutes))),
        };

        return await Converter.ConvertToDomain(Dao.FindBy(new StatementParameters { Where = parameters }, cancellationToken), cancellationToken);
    }

    public async Task<Cart> AddAsync(Cart entity, CancellationToken cancellationToken = default) {
        if (entity.Customer != null) {
            await AddOrUpdateCustomer(entity, cancellationToken);
        }
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.AddAsync(dataModel, cancellationToken);
        await Converter.CartItemRepository.AddAsync(entity.Items, cancellationToken);
        entity = Converter.ApplyDataModel(entity, dataModel);
        return entity;
    }

    public async Task<Cart> UpdateAsync(Cart oldEntity, Cart newEntity, CancellationToken cancellationToken = default) {
        await Converter.CartItemRepository.UpdateProductsAsync(oldEntity.Items, newEntity.Items, cancellationToken);
        if (newEntity.Customer != null) {
            await AddOrUpdateCustomer(newEntity, cancellationToken);
        }
        var dataModel = Converter.ConvertFromDomain(newEntity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        newEntity = Converter.ApplyDataModel(newEntity, dataModel);
        return newEntity;
    }

    public async Task DeleteAsync(Cart entity, CancellationToken cancellationToken = default) {
        await Dao.DeleteAsync(Converter.ConvertFromDomain(entity), cancellationToken);
    }
    
    public async Task DeleteAsync(IEnumerable<Cart> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.DeleteAsync(dataModels, cancellationToken);
    }

    private async Task<Cart> AddOrUpdateCustomer(Cart cart, CancellationToken cancellationToken = default) {
        if (cart.Customer == null) {
            return null!;
        }
        if (cart.Customer.ShopId == Guid.Empty) {
            cart = cart with {
                Customer = cart.Customer with {
                    ShopId = cart.ShopId
                }
            };
        }
        var oldCustomer = await Converter.CustomerRepository.FindByIdAsync(cart.Customer.Id, cancellationToken);
        Customer newCustomer;
        if (oldCustomer == null) {
            newCustomer = await Converter.CustomerRepository.AddAsync(cart.Customer, cancellationToken);
        } else {
            newCustomer = await Converter.CustomerRepository.UpdateAsync(oldCustomer, cart.Customer, cancellationToken);
        }
        return cart with {
            Customer = newCustomer
        };
    }

    private class CartDomainModelConvert : IDomainReadModelConverter<CartDataModel, Cart> {
        internal CartItemRepository CartItemRepository { get; }
        internal ICouponRepository CouponRepository { get; }
        internal ICustomerRepository CustomerRepository { get; }
        internal ISystemClock TimeProvider { get; }

        public CartDomainModelConvert(IDao<ProductCartDataModel> cartItemDao, IProductRepository productRepository, 
            ICustomerRepository customerRepository, ISystemClock timeProvider, ICouponRepository couponRepository) {
            CartItemRepository = new CartItemRepository(cartItemDao, productRepository);
            CustomerRepository = customerRepository;
            TimeProvider = timeProvider;
            CouponRepository = couponRepository;
        }

        public Cart ApplyDataModel(Cart domainModel, CartDataModel dataModel) {
            return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
        }
        
        public IReadOnlyList<CartDataModel> ConvertFromDomain(IEnumerable<Cart> domainModels)
            => domainModels.Select(ConvertFromDomain).ToList();

        public CartDataModel ConvertFromDomain(Cart domainModel) {
            return new CartDataModel() {
                Id = domainModel.Id,
                CustomerId = domainModel.Customer?.Id,
                ShopId = domainModel.ShopId,
                LastAccess = domainModel.LastAccess,
                RowVersion = domainModel.GetRowVersion(),
                CreationTime = TimeProvider.UtcNow,
                LastModificationTime = TimeProvider.UtcNow
            };
        }

        public async ValueTask<Cart> ConvertToDomain(CartDataModel dataModel, CancellationToken cancellationToken) {
            return (await ConvertToDomain(ImmutableList.Create(dataModel), cancellationToken))[0];
        }

        public async Task<IReadOnlyList<Cart>> ConvertToDomain(IAsyncEnumerable<CartDataModel> dataModels, CancellationToken cancellationToken = default) {
            var items = await dataModels.ToListAsync(cancellationToken);
            return await ConvertToDomain(items, cancellationToken);
        }

        private async Task<IReadOnlyList<Cart>> ConvertToDomain(IReadOnlyCollection<CartDataModel> dataModels,
            CancellationToken cancellationToken = default) {
            var cartIds = dataModels.Select(p => p.Id).ToHashSet();
            var customerIds = dataModels.Select(p => p.CustomerId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value).ToHashSet();
            var cartItemsDict = await CartItemRepository.FindByCartIds(cartIds, cancellationToken);
            var customerDict = (await CustomerRepository
                    .FindByIdsAsync(customerIds, cancellationToken))
                .ToDictionary(s => s.Id, s => s);
            var couponsDict = await CouponRepository.FindByCartIdsAsync(cartIds, cancellationToken);
            var domainModels = ImmutableList.CreateBuilder<Cart>();
            foreach (var dataModel in dataModels) {
                var cartItems = cartItemsDict.TryGetValue(dataModel.Id, out var cartItemsList)
                    ? cartItemsList.ToImmutableArray()
                    : ImmutableArray<CartItem>.Empty;
                var customer = dataModel.CustomerId.HasValue ? customerDict.GetValueOrDefault(dataModel.CustomerId.Value) : null;
                var coupons = couponsDict.TryGetValue(dataModel.Id, out var couponsList) ? 
                    couponsList.ToImmutableArray() : ImmutableArray<Coupon>.Empty;
                domainModels.Add(new Cart() {
                    Id = dataModel.Id,
                    Customer = customer,
                    ShopId = dataModel.ShopId,
                    Items = cartItems,
                    Coupons = coupons,
                    LastAccess = dataModel.LastAccess,
                    ConcurrencyToken = dataModel.GetConcurrencyToken()
                });
            }
            return domainModels.ToImmutable();
        }
    }
}