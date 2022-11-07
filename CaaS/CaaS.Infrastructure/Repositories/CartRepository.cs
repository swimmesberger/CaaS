using System.Collections.Immutable;
using CaaS.Core.Entities;
using CaaS.Core.Repositories;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories.Base;

namespace CaaS.Infrastructure.Repositories; 

public class CartRepository : CrudReadRepository<CartDataModel, Cart>, ICartRepository {
    internal new CartDomainModelConvert Converter => (CartDomainModelConvert)base.Converter;
    
    public CartRepository(IDao<CartDataModel> dao, IDao<ProductCartDataModel> cartItemDao, IShopRepository shopRepository, 
            IProductRepository productRepository, ICustomerRepository customerRepository) : 
            base(dao, new CartDomainModelConvert(cartItemDao, shopRepository, productRepository, customerRepository)) { }

    public async Task<Cart?> FindCartByCustomerId(Guid customerId, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindBy(StatementParameters.CreateWhere(nameof(CartDataModel.CustomerId), customerId), cancellationToken)
                .FirstOrDefaultAsync(cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<Cart> AddAsync(Cart entity, CancellationToken cancellationToken = default) {
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
    
    public async Task<Cart> UpdateAsync(Cart entity, CancellationToken cancellationToken = default) {
        await UpdateProductsAsync(entity, cancellationToken);
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }
    
    public Task DeleteAsync(Cart entity, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }

    private async Task UpdateProductsAsync(Cart entity, CancellationToken cancellationToken = default) {
        var currentItemsDict = (await Converter.CartItemRepository
                .FindByCartId(entity.Id, cancellationToken)).ToDictionary(i => i.Id);
        var addedItems = entity.Items.Where(cartItem => !currentItemsDict.ContainsKey(cartItem.Id));
        var updatedItems = entity.Items.Where(cartItem => currentItemsDict.ContainsKey(cartItem.Id));
        var removedItems = currentItemsDict.Values.Except(entity.Items, CartItemIdComparer.Instance);
        await Converter.CartItemRepository.AddAsync(addedItems, cancellationToken);
        await Converter.CartItemRepository.UpdateAsync(updatedItems, cancellationToken);
        await Converter.CartItemRepository.DeleteAsync(removedItems, cancellationToken);
    }

    private class CartItemIdComparer : IEqualityComparer<CartItem> {
        public static readonly CartItemIdComparer Instance = new CartItemIdComparer();
        
        public bool Equals(CartItem? x, CartItem? y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id.Equals(y.Id);
        }
        
        public int GetHashCode(CartItem obj) => obj.Id.GetHashCode();
    }
}

internal class CartDomainModelConvert : IDomainReadModelConverter<CartDataModel, Cart> {
    public IEnumerable<OrderParameter>? DefaultOrderParameters => null;

    internal CartItemRepository CartItemRepository { get; }
    internal IShopRepository ShopRepository { get; }
    internal ICustomerRepository CustomerRepository { get; }

    public CartDomainModelConvert(IDao<ProductCartDataModel> cartItemDao, IShopRepository shopRepository, 
            IProductRepository productRepository, ICustomerRepository customerRepository) {
        CartItemRepository = new CartItemRepository(cartItemDao, productRepository);
        ShopRepository = shopRepository;
        CustomerRepository = customerRepository;
    }
    
    public CartDataModel ConvertFromDomain(Cart domainModel) {
        return new CartDataModel() {
            Id = domainModel.Id,
            CustomerId = domainModel.Customer?.Id,
            ShopId = domainModel.ShopId,
            LastAccess = domainModel.LastAccess,
            RowVersion = domainModel.GetRowVersion()
        };
    }

    public async ValueTask<Cart> ConvertToDomain(CartDataModel dataModel, CancellationToken cancellationToken) {
        return (await ConvertToDomain(new List<CartDataModel>() { dataModel }, cancellationToken)).First();
    }
    
    public async Task<List<Cart>> ConvertToDomain(IAsyncEnumerable<CartDataModel> dataModels, CancellationToken cancellationToken = default) {
        var items = await dataModels.ToListAsync(cancellationToken);
        return await ConvertToDomain(items, cancellationToken);
    }
    
    private async Task<List<Cart>> ConvertToDomain(IReadOnlyCollection<CartDataModel> dataModels, CancellationToken cancellationToken = default) {
        var cartIds = dataModels.Select(p => p.Id).ToHashSet();
        var customerIds = dataModels.Select(p => p.CustomerId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value).ToHashSet();
        var cartItemsDict = await CartItemRepository.FindByCartIds(cartIds, cancellationToken);
        var customerDict = (await CustomerRepository
                .FindByIdsAsync(customerIds, cancellationToken))
                .ToDictionary(s => s.Id, s => s);
        var domainModels = new List<Cart>();
        foreach (var dataModel in dataModels) {
            var cartItems = cartItemsDict.TryGetValue(dataModel.Id, out var cartItemsList) ? 
                    cartItemsList.ToImmutableList() : ImmutableList<CartItem>.Empty;
            var customer = dataModel.CustomerId.HasValue ? customerDict.GetValueOrDefault(dataModel.CustomerId.Value) : null;
            domainModels.Add(new Cart() {
                Id = dataModel.Id,
                Customer = customer,
                ShopId = dataModel.ShopId,
                Items = cartItems,
                LastAccess = dataModel.LastAccess,
                ConcurrencyToken = dataModel.GetConcurrencyToken()
            });
        }
        return domainModels;
    }
}