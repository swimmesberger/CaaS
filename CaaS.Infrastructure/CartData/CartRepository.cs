using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.CartAggregate;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.ProductAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Ado.Model.Where;
using CaaS.Infrastructure.Base.Repository;
using CaaS.Infrastructure.CouponData;

namespace CaaS.Infrastructure.CartData; 

public class CartRepository : CrudReadRepository<CartDataModel, Cart>, ICartRepository {
    private new CartDomainModelConvert Converter => (CartDomainModelConvert)base.Converter;
    
    public CartRepository(IDao<CartDataModel> dao, IDao<ProductCartDataModel> cartItemDao, 
            IProductRepository productRepository, ICustomerRepository customerRepository, ICouponRepository couponRepository, IDateTimeOffsetProvider timeProvider) : 
            base(dao, new CartDomainModelConvert(cartItemDao, productRepository, customerRepository, timeProvider, couponRepository)) {
    }

    public async Task<Cart?> FindCartByCustomerId(Guid customerId, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindBy(StatementParameters.CreateWhere(nameof(CartDataModel.CustomerId), customerId), cancellationToken)
                .FirstOrDefaultAsync(cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }
    
    public async Task<IReadOnlyList<Cart>> FindCartsByShopId(Guid shopId, CancellationToken cancellationToken = default) {
        return await Converter
            .ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(Cart.ShopId), shopId), cancellationToken), cancellationToken);
    }

    public async Task<IReadOnlyList<Cart>> FindExpiredCarts(int lifeTimeMinutes, CancellationToken cancellationToken = default) {
        var parameters = new List<QueryParameter> {
            QueryParameter.From(nameof(Cart.LastAccess), Converter.TimeProvider.GetNow().Subtract(TimeSpan.FromMinutes(lifeTimeMinutes)), comparator: WhereComparator.Less),
        };

        return await Converter.ConvertToDomain(Dao.FindBy(StatementParameters.CreateWhere(parameters), cancellationToken), cancellationToken);
    }

    public async Task<Cart> AddAsync(Cart entity, CancellationToken cancellationToken = default) {
        await Converter.CartItemRepository.AddAsync(entity.Items, cancellationToken);
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.AddAsync(dataModel, cancellationToken);
        entity = Converter.ApplyDataModel(entity, dataModel);
        return entity;
    }

    public async Task<Cart> UpdateAsync(Cart oldEntity, Cart newEntity, CancellationToken cancellationToken = default) {
        await Converter.CartItemRepository.UpdateProductsAsync(oldEntity.Items, newEntity.Items, cancellationToken);
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
    
    private class CartDomainModelConvert : IDomainReadModelConverter<CartDataModel, Cart> {
        public IEnumerable<OrderParameter>? DefaultOrderParameters => null;

        internal CartItemRepository CartItemRepository { get; }
        internal ICouponRepository CouponRepository { get; }
        private ICustomerRepository CustomerRepository { get; }
        internal IDateTimeOffsetProvider TimeProvider { get; }

        public CartDomainModelConvert(IDao<ProductCartDataModel> cartItemDao, IProductRepository productRepository, 
            ICustomerRepository customerRepository, IDateTimeOffsetProvider timeProvider, ICouponRepository couponRepository) {
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
                CreationTime = TimeProvider.GetNow(),
                LastModificationTime = TimeProvider.GetNow()
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
            var couponsDict = await CouponRepository.FindByCartIds(cartIds, cancellationToken);
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