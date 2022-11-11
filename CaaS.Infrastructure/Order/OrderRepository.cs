using System.Collections.Immutable;
using CaaS.Core.Coupon;
using CaaS.Core.Customer;
using CaaS.Core.Exceptions;
using CaaS.Core.Order;
using CaaS.Core.Order.Entities;
using CaaS.Core.Product;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Repository;
using CaaS.Infrastructure.Order.DataModel;

namespace CaaS.Infrastructure.Order; 

public class OrderRepository : CrudReadRepository<OrderDataModel, Core.Order.Entities.Order>, IOrderRepository {
    internal new OrderDomainModelConvert Converter => (OrderDomainModelConvert)base.Converter;
    
    public OrderRepository(IDao<OrderDataModel> dao, 
                            IDao<ProductOrderDataModel> orderItemDao,
                            IDao<ProductOrderDiscountDataModel> orderItemDiscountDao,
                            IDao<OrderDiscountDataModel> orderDiscountDao,
                            IProductRepository productRepository, 
                            ICustomerRepository customerRepository,
                            ICouponRepository couponRepository) : 
                            base(dao, new OrderDomainModelConvert(
                                                        orderItemDao,
                                                        orderItemDiscountDao,
                                                        orderDiscountDao,
                                                        productRepository,
                                                        customerRepository,
                                                        couponRepository)) { }
    
    public async Task<IReadOnlyList<Core.Order.Entities.Order>> FindByCustomerId(Guid customerId, CancellationToken cancellationToken = default) {
        return await Converter
            .ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(OrderDataModel.CustomerId), customerId), cancellationToken), cancellationToken);
    }
    
    public async Task<Core.Order.Entities.Order> AddAsync(Core.Order.Entities.Order entity, CancellationToken cancellationToken = default) {
        await Converter.OrderItemRepository.AddAsync(entity.Items, cancellationToken);
        await Converter.OrderDiscountRepository.AddAsync(entity.OrderDiscounts, cancellationToken);
        await Converter.CouponRepository.AddAsync(entity.Coupons);
            
        var dataModel =  new OrderDataModel() {
            Id = entity.Id,
            ShopId = entity.ShopId,
            CustomerId = entity.Customer.Id,
            OrderNumber = entity.OrderNumber,
            OrderDate = entity.OrderDate,
            RowVersion = entity.GetRowVersion()
        };
        dataModel = await Dao.AddAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }
    
    public async Task AddAsync(IEnumerable<Core.Order.Entities.Order> entities, CancellationToken cancellationToken = default) {
        var domainModels = entities.ToList();
        var orderItems = domainModels.SelectMany(o => o.Items);
        await Converter.OrderItemRepository.AddAsync(orderItems, cancellationToken);

        var orderDiscounts = domainModels.SelectMany(o => o.OrderDiscounts);
        await Converter.OrderDiscountRepository.AddAsync(orderDiscounts);

        var coupons = domainModels.SelectMany(o => o.Coupons);
        await Converter.CouponRepository.AddAsync(coupons);

        var dataModels = Converter.ConvertFromDomain(domainModels);
        await Dao.AddAsync(dataModels, cancellationToken);
    }
    
    public async Task<Core.Order.Entities.Order> UpdateAsync(Core.Order.Entities.Order oldEntity, Core.Order.Entities.Order newEntity, CancellationToken cancellationToken = default) {
        await Converter.OrderItemRepository.UpdateOrderItemsAsync(oldEntity.Items, newEntity.Items, cancellationToken);
        await Converter.CouponRepository.UpdateAsync(oldEntity.Coupons, newEntity.Coupons, cancellationToken);
        //Todo: order discounts updaten
        return await UpdateImplAsync(newEntity, cancellationToken);
    }

    public async Task UpdateAsync(IEnumerable<Core.Order.Entities.Order> entities, CancellationToken cancellationToken = default) {
        //Todo: orderItems updaten
        //Todo: coupons updaten
        //Todo: order discounts updaten
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.UpdateAsync(dataModels, cancellationToken);
    }
    private async Task<Core.Order.Entities.Order> UpdateImplAsync(Core.Order.Entities.Order entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }

    
    public async Task DeleteAsync(Core.Order.Entities.Order entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        await Dao.DeleteAsync(dataModel, cancellationToken);
    }
    
    public async Task DeleteAsync(IEnumerable<Core.Order.Entities.Order> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.DeleteAsync(dataModels, cancellationToken);
    }
}

internal class OrderDomainModelConvert : IDomainReadModelConverter<OrderDataModel, Core.Order.Entities.Order> {
    public IEnumerable<OrderParameter>? DefaultOrderParameters => null;
    
    internal OrderItemRepository OrderItemRepository { get; }
    internal ICustomerRepository CustomerRepository { get; }
    internal ICouponRepository CouponRepository { get; }
    internal OrderDiscountRepository OrderDiscountRepository { get; }
    
    public OrderDomainModelConvert(
                IDao<ProductOrderDataModel> productOrderDao, 
                IDao<ProductOrderDiscountDataModel> orderItemDiscountDao, 
                IDao<OrderDiscountDataModel> orderDiscountDao,
                IProductRepository productRepository,
                ICustomerRepository customerRepository,
                ICouponRepository couponRepository) {
        OrderItemRepository = new OrderItemRepository(productOrderDao, productRepository, orderItemDiscountDao);
        CustomerRepository = customerRepository;
        CouponRepository = couponRepository;
        OrderDiscountRepository = new OrderDiscountRepository(orderDiscountDao);
    }

    public IReadOnlyList<OrderDataModel> ConvertFromDomain(IEnumerable<Core.Order.Entities.Order> domainModels)
        => domainModels.Select(ConvertFromDomain).ToList();
    
    public OrderDataModel ConvertFromDomain(Core.Order.Entities.Order domainModel) {
        return new OrderDataModel {
            Id = domainModel.Id,
            OrderNumber = domainModel.OrderNumber,
            OrderDate = domainModel.OrderDate,
            CustomerId = domainModel.Customer.Id,
            ShopId = domainModel.ShopId,
            RowVersion = domainModel.GetRowVersion()
        };
    }
    public async ValueTask<Core.Order.Entities.Order> ConvertToDomain(OrderDataModel dataModel, CancellationToken cancellationToken) {
        return (await ConvertToDomain(new List<OrderDataModel>() { dataModel }, cancellationToken)).First();
    }
    public async Task<IReadOnlyList<Core.Order.Entities.Order>> ConvertToDomain(IAsyncEnumerable<OrderDataModel> dataModels, CancellationToken cancellationToken = default) {
        var items = await dataModels.ToListAsync(cancellationToken);
        return await ConvertToDomain(items, cancellationToken);
    }
    
    private async Task<IReadOnlyList<Core.Order.Entities.Order>> ConvertToDomain(IReadOnlyCollection<OrderDataModel> dataModels, CancellationToken cancellationToken = default) {
        var orderIds = dataModels.Select(p => p.Id).ToHashSet();
        var customerIds = dataModels.Select(p => p.CustomerId).ToHashSet();
        
        var couponsDict = await CouponRepository.FindByOrderIds(orderIds, cancellationToken);
        var orderDiscountsDict = await OrderDiscountRepository.FindByOrderIdsAsync(orderIds, cancellationToken);
        var orderItemsDict = await OrderItemRepository.FindByOrderIds(orderIds, cancellationToken);
        var customerDict = (await CustomerRepository
                .FindByIdsAsync(customerIds, cancellationToken))
            .ToDictionary(s => s.Id, s => s);
        var domainModels = new List<Core.Order.Entities.Order>();
        foreach (var dataModel in dataModels) {
            var orderDiscounts = orderDiscountsDict.TryGetValue(dataModel.Id, out var orderDiscountsList) ? 
                orderDiscountsList.ToImmutableArray() : ImmutableArray<OrderDiscount>.Empty;
            var orderItems = orderItemsDict.TryGetValue(dataModel.Id, out var orderItemsList) ? 
                orderItemsList.ToImmutableArray() : ImmutableArray<OrderItem>.Empty;
            var coupons = couponsDict.TryGetValue(dataModel.Id, out var couponsList) ? 
                couponsList.ToImmutableArray() : ImmutableArray<Core.Coupon.Entities.Coupon>.Empty;
            var customer = customerDict.GetValueOrDefault(dataModel.CustomerId);
            if (customer == null) {
                throw new CaasDomainMappingException($"Failed to find customer {dataModel.CustomerId} for order {dataModel.Id}");
            }
            domainModels.Add(new Core.Order.Entities.Order() {
                Id = dataModel.Id,
                Customer = customer,
                ShopId = dataModel.ShopId,
                Items = orderItems,
                OrderDate = dataModel.OrderDate,
                OrderNumber = dataModel.OrderNumber,
                Coupons = coupons,
                OrderDiscounts = orderDiscounts,
                ConcurrencyToken = dataModel.GetConcurrencyToken()
            });
        }
        return domainModels;
    }
}