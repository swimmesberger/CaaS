using System.Collections.Immutable;
using CaaS.Core.Cart;
using CaaS.Core.Cart.Entities;
using CaaS.Core.Customer;
using CaaS.Core.Product;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Repository;
using CaaS.Infrastructure.Cart.DataModel;

namespace CaaS.Infrastructure.Cart; 

public class CartRepository : CrudReadRepository<CartDataModel, Core.Cart.Entities.Cart>, ICartRepository {
    private new CartDomainModelConvert Converter => (CartDomainModelConvert)base.Converter;
    
    public CartRepository(IDao<CartDataModel> dao, IDao<ProductCartDataModel> cartItemDao, 
            IProductRepository productRepository, ICustomerRepository customerRepository) : 
            base(dao, new CartDomainModelConvert(cartItemDao, productRepository, customerRepository)) { }

    public async Task<Core.Cart.Entities.Cart?> FindCartByCustomerId(Guid customerId, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindBy(StatementParameters.CreateWhere(nameof(CartDataModel.CustomerId), customerId), cancellationToken)
                .FirstOrDefaultAsync(cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<Core.Cart.Entities.Cart> AddAsync(Core.Cart.Entities.Cart entity, CancellationToken cancellationToken = default) {
        await Converter.CartItemRepository.AddAsync(entity.Items, cancellationToken);
        var dataModel =  new CartDataModel() {
            Id = entity.Id,
            ShopId = entity.ShopId,
            CustomerId = entity.Customer?.Id,
            LastAccess = entity.LastAccess,
            RowVersion = entity.GetRowVersion()
        };
        dataModel = await Dao.AddAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }
    
    public async Task<Core.Cart.Entities.Cart> UpdateAsync(Core.Cart.Entities.Cart entity, CancellationToken cancellationToken = default) {
        var oldItems = await Converter.CartItemRepository
            .FindByCartId(entity.Id, cancellationToken);
        await Converter.CartItemRepository.UpdateProductsAsync(oldItems, entity.Items, cancellationToken);
        return await UpdateImplAsync(entity, cancellationToken);
    }

    public async Task<Core.Cart.Entities.Cart> UpdateAsync(Core.Cart.Entities.Cart oldEntity, Core.Cart.Entities.Cart newEntity, CancellationToken cancellationToken = default) {
        await Converter.CartItemRepository.UpdateProductsAsync(oldEntity.Items, newEntity.Items, cancellationToken);
        return await UpdateImplAsync(newEntity, cancellationToken);
    }

    public async Task DeleteAsync(Core.Cart.Entities.Cart entity, CancellationToken cancellationToken = default) {
        await Dao.DeleteAsync(Converter.ConvertFromDomain(entity), cancellationToken);
    }

    private async Task<Core.Cart.Entities.Cart> UpdateImplAsync(Core.Cart.Entities.Cart entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }

    private class CartDomainModelConvert : IDomainReadModelConverter<CartDataModel, Core.Cart.Entities.Cart> {
        public IEnumerable<OrderParameter>? DefaultOrderParameters => null;

        internal CartItemRepository CartItemRepository { get; }
        private ICustomerRepository CustomerRepository { get; }

        public CartDomainModelConvert(IDao<ProductCartDataModel> cartItemDao, IProductRepository productRepository, 
            ICustomerRepository customerRepository) {
            CartItemRepository = new CartItemRepository(cartItemDao, productRepository);
            CustomerRepository = customerRepository;
        }

        public CartDataModel ConvertFromDomain(Core.Cart.Entities.Cart domainModel) {
            return new CartDataModel() {
                Id = domainModel.Id,
                CustomerId = domainModel.Customer?.Id,
                ShopId = domainModel.ShopId,
                LastAccess = domainModel.LastAccess,
                RowVersion = domainModel.GetRowVersion()
            };
        }

        public async ValueTask<Core.Cart.Entities.Cart> ConvertToDomain(CartDataModel dataModel, CancellationToken cancellationToken) {
            return (await ConvertToDomain(ImmutableList.Create(dataModel), cancellationToken))[0];
        }

        public async Task<IReadOnlyList<Core.Cart.Entities.Cart>> ConvertToDomain(IAsyncEnumerable<CartDataModel> dataModels, CancellationToken cancellationToken = default) {
            var items = await dataModels.ToListAsync(cancellationToken);
            return await ConvertToDomain(items, cancellationToken);
        }

        private async Task<IReadOnlyList<Core.Cart.Entities.Cart>> ConvertToDomain(IReadOnlyCollection<CartDataModel> dataModels,
            CancellationToken cancellationToken = default) {
            var cartIds = dataModels.Select(p => p.Id).ToHashSet();
            var customerIds = dataModels.Select(p => p.CustomerId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value).ToHashSet();
            var cartItemsDict = await CartItemRepository.FindByCartIds(cartIds, cancellationToken);
            var customerDict = (await CustomerRepository
                    .FindByIdsAsync(customerIds, cancellationToken))
                .ToDictionary(s => s.Id, s => s);
            var domainModels = ImmutableList.CreateBuilder<Core.Cart.Entities.Cart>();
            foreach (var dataModel in dataModels) {
                var cartItems = cartItemsDict.TryGetValue(dataModel.Id, out var cartItemsList)
                    ? cartItemsList.ToImmutableArray()
                    : ImmutableArray<CartItem>.Empty;
                var customer = dataModel.CustomerId.HasValue ? customerDict.GetValueOrDefault(dataModel.CustomerId.Value) : null;
                domainModels.Add(new Core.Cart.Entities.Cart() {
                    Id = dataModel.Id,
                    Customer = customer,
                    ShopId = dataModel.ShopId,
                    Items = cartItems,
                    LastAccess = dataModel.LastAccess,
                    ConcurrencyToken = dataModel.GetConcurrencyToken()
                });
            }
            return domainModels.ToImmutable();
        }
    }
}