using System.Collections.Immutable;
using CaaS.Core.Entities;
using CaaS.Core.Repositories;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories.Base;

namespace CaaS.Infrastructure.Repositories; 

public class OrderRepository : CrudReadRepository<OrderDataModel, Order>, IOrderRepository {
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
    
    public async Task<IReadOnlyList<Order>> FindOrdersByCustomerId(Guid customerId, CancellationToken cancellationToken = default) {
        return await Converter
            .ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(OrderDataModel.CustomerId), customerId), cancellationToken), cancellationToken);
    }
    
    public async Task<Order> AddAsync(Order entity, CancellationToken cancellationToken = default) {
        await Converter.OrderItemRepository.AddAsync(entity.Items, cancellationToken);
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
    
    public async Task<Order> UpdateAsync(Order oldEntity, Order newEntity, CancellationToken cancellationToken = default) {
        await Converter.OrderItemRepository.UpdateOrderItemsAsync(oldEntity.Items, newEntity.Items, cancellationToken);
        return await UpdateImplAsync(newEntity, cancellationToken);
    }
    
    private async Task<Order> UpdateImplAsync(Order entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }
    
    public async Task DeleteAsync(Order entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        await Dao.DeleteAsync(dataModel, cancellationToken);
    }
}

internal class OrderDomainModelConvert : IDomainReadModelConverter<OrderDataModel, Order> {
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

    public OrderDataModel ConvertFromDomain(Order domainModel) {
        return new OrderDataModel {
            Id = domainModel.Customer.Id,
            OrderNumber = domainModel.OrderNumber,
            OrderDate = domainModel.OrderDate,
            CustomerId = domainModel.Customer.Id,
            RowVersion = domainModel.GetRowVersion()
        };
    }
    public async ValueTask<Order> ConvertToDomain(OrderDataModel dataModel, CancellationToken cancellationToken) {
        return (await ConvertToDomain(new List<OrderDataModel>() { dataModel }, cancellationToken)).First();
    }
    public async Task<IReadOnlyList<Order>> ConvertToDomain(IAsyncEnumerable<OrderDataModel> dataModels, CancellationToken cancellationToken = default) {
        var items = await dataModels.ToListAsync(cancellationToken);
        return await ConvertToDomain(items, cancellationToken);
    }
    
    private async Task<IReadOnlyList<Order>> ConvertToDomain(IReadOnlyCollection<OrderDataModel> dataModels, CancellationToken cancellationToken = default) {
        var orderIds = dataModels.Select(p => p.Id).ToHashSet();
        var customerIds = dataModels.Select(p => p.CustomerId).ToHashSet();
        
        var couponsDict = await CouponRepository.FindByOrderIds(orderIds, cancellationToken);
        var orderDiscountsDict = await OrderDiscountRepository.FindByOrderIdsAsync(orderIds, cancellationToken);
        var orderItemsDict = await OrderItemRepository.FindByOrderIds(orderIds, cancellationToken);
        var customerDict = (await CustomerRepository
                .FindByIdsAsync(customerIds, cancellationToken))
            .ToDictionary(s => s.Id, s => s);
        var domainModels = new List<Order>();
        foreach (var dataModel in dataModels) {
            var orderDiscounts = orderDiscountsDict.TryGetValue(dataModel.Id, out var orderDiscountsList) ? 
                orderDiscountsList.ToImmutableList() : ImmutableList<OrderDiscount>.Empty;
            var orderItems = orderItemsDict.TryGetValue(dataModel.Id, out var orderItemsList) ? 
                orderItemsList.ToImmutableList() : ImmutableList<OrderItem>.Empty;
            var coupons = couponsDict.TryGetValue(dataModel.Id, out var couponsLIst) ? 
                couponsLIst.ToImmutableList() : ImmutableList<Coupon>.Empty;
            var customer = customerDict.GetValueOrDefault(dataModel.CustomerId);
            domainModels.Add(new Order() {
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