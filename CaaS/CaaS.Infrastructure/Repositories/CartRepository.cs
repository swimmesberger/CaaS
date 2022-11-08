﻿using System.Collections.Immutable;
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
        var oldItems = await Converter.CartItemRepository
            .FindByCartId(entity.Id, cancellationToken);
        await Converter.CartItemRepository.UpdateProductsAsync(oldItems, entity.Items, cancellationToken);
        return await UpdateImplAsync(entity, cancellationToken);
    }

    public async Task<Cart> UpdateAsync(Cart oldEntity, Cart newEntity, CancellationToken cancellationToken = default) {
        await Converter.CartItemRepository.UpdateProductsAsync(oldEntity.Items, newEntity.Items, cancellationToken);
        return await UpdateImplAsync(newEntity, cancellationToken);
    }

    public async Task DeleteAsync(Cart entity, CancellationToken cancellationToken = default) {
        await Dao.DeleteAsync(Converter.ConvertFromDomain(entity), cancellationToken);
    }

    private async Task<Cart> UpdateImplAsync(Cart entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
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
            return (await ConvertToDomain(ImmutableList.Create(dataModel), cancellationToken))[0];
        }

        public async Task<IReadOnlyList<Cart>> ConvertToDomain(IAsyncEnumerable<CartDataModel> dataModels, CancellationToken cancellationToken = default) {
            var items = await dataModels.ToImmutableArrayAsync(cancellationToken);
            return await ConvertToDomain(items, cancellationToken);
        }

        private async Task<IReadOnlyList<Cart>> ConvertToDomain(IReadOnlyCollection<CartDataModel> dataModels,
            CancellationToken cancellationToken = default) {
            var cartIds = dataModels.Select(p => p.Id).ToImmutableHashSet();
            var customerIds = dataModels.Select(p => p.CustomerId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value).ToImmutableHashSet();
            var cartItemsDict = await CartItemRepository.FindByCartIds(cartIds, cancellationToken);
            var customerDict = (await CustomerRepository
                    .FindByIdsAsync(customerIds, cancellationToken))
                .ToImmutableDictionary(s => s.Id, s => s);
            var domainModels = ImmutableList.CreateBuilder<Cart>();
            foreach (var dataModel in dataModels) {
                var cartItems = cartItemsDict.TryGetValue(dataModel.Id, out var cartItemsList)
                    ? cartItemsList.ToImmutableArray()
                    : ImmutableArray<CartItem>.Empty;
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
            return domainModels.ToImmutable();
        }
    }
}