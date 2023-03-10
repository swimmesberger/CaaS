using System.Collections.Immutable;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.DiscountAggregate.Models;
using CaaS.Core.OrderAggregate;
using CaaS.Core.ProductAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Ado.Query.Parameters.Where;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.OrderData; 

public class OrderRepository : CrudReadRepository<OrderDataModel, Order>, IOrderRepository {
    private new OrderDomainModelConvert Converter => (OrderDomainModelConvert)base.Converter;
    
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
    
    public async Task<IReadOnlyList<Order>> FindByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default) {
        return await Converter
            .ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(OrderDataModel.CustomerId), customerId), cancellationToken), cancellationToken);
    }
    
    public async Task<Order> AddAsync(Order entity, Address address, CancellationToken cancellationToken = default) {
        var dataModel =  new OrderDataModel() {
            Id = entity.Id,
            ShopId = entity.ShopId,
            CustomerId = entity.Customer.Id,
            OrderDate = entity.OrderDate,
            AddressStreet = address.Street,
            AddressCity = address.City,
            AddressZipCode = address.ZipCode,
            AddressState = address.State,
            AddressCountry = address.Country,
            RowVersion = entity.GetRowVersion()
        };

        dataModel = await Dao.AddAsync(dataModel, cancellationToken);
        
        await Converter.OrderItemRepository.AddAsync(entity.Items, cancellationToken);
        await Converter.OrderDiscountRepository.AddAsync(entity.OrderDiscounts, cancellationToken);

        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }
    
    public async Task<IReadOnlyList<Order>> FindByDateRangeAsync(DateTimeOffset from, DateTimeOffset until, CancellationToken cancellationToken = default) {
        var parameters = new List<QueryParameter> {
            new (nameof(OrderDataModel.CreationTime), WhereComparator.GreaterOrEqual, from),
            new (nameof(OrderDataModel.CreationTime), WhereComparator.LessOrEqual, until),
        };
        
        return (await Converter.ConvertToDomain(Dao
                .FindBy(new StatementParameters { Where = parameters }, cancellationToken), cancellationToken))
            .ToList();
    }

    public async Task<int> FindOrderNumberById(Guid orderId, CancellationToken cancellationToken = default) {
        var parameters = new StatementParameters {
            SelectParameters = new SelectParameters("OrderNumber"),
            WhereParameters = new WhereParameters(new QueryParameter[] {
              new(nameof(OrderDataModel.Id), orderId)  
            })
        };
        
        var result = await Dao.FindScalarBy<int>(parameters, cancellationToken).ToListAsync(cancellationToken);
        if (result.Count == 0) {
            throw new CaasItemNotFoundException($"order '{orderId}' does not exist");
        }

        return result[0];
    }

    public async Task<Order> UpdateAsync(Order oldEntity, Order newEntity, CancellationToken cancellationToken = default) {
        await Converter.OrderItemRepository.UpdateOrderItemsAsync(oldEntity.Items, newEntity.Items, cancellationToken);
        await Converter.CouponRepository.UpdateAsync(oldEntity.Coupons, newEntity.Coupons, cancellationToken);
        await Converter.OrderDiscountRepository.UpdateAsync(oldEntity.OrderDiscounts, newEntity.OrderDiscounts, cancellationToken);
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

    public IReadOnlyList<OrderDataModel> ConvertFromDomain(IEnumerable<Order> domainModels)
        => domainModels.Select(ConvertFromDomain).ToList();
    
    public OrderDataModel ConvertFromDomain(Order domainModel) {
        return new OrderDataModel {
            Id = domainModel.Id,
            OrderNumber = domainModel.OrderNumber,
            OrderDate = domainModel.OrderDate,
            CustomerId = domainModel.Customer.Id,
            ShopId = domainModel.ShopId,
            AddressCountry = domainModel.Address.Country,
            AddressCity = domainModel.Address.City,
            AddressState = domainModel.Address.State,
            AddressStreet = domainModel.Address.Street,
            AddressZipCode = domainModel.Address.ZipCode,
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
        
        var couponsDict = await CouponRepository.FindByOrderIdsAsync(orderIds, cancellationToken);
        var orderDiscountsDict = await OrderDiscountRepository.FindByOrderIdsAsync(orderIds, cancellationToken);
        var orderItemsDict = await OrderItemRepository.FindByOrderIds(orderIds, cancellationToken);
        var customerDict = (await CustomerRepository
                .FindByIdsAsync(customerIds, cancellationToken))
            .ToDictionary(s => s.Id, s => s);
        var domainModels = new List<Order>();
        foreach (var dataModel in dataModels) {
            var orderDiscounts = orderDiscountsDict.TryGetValue(dataModel.Id, out var orderDiscountsList) ? 
                orderDiscountsList.ToImmutableArray() : ImmutableArray<Discount>.Empty;
            var orderItems = orderItemsDict.TryGetValue(dataModel.Id, out var orderItemsList) ? 
                orderItemsList.ToImmutableArray() : ImmutableArray<OrderItem>.Empty;
            var coupons = couponsDict.TryGetValue(dataModel.Id, out var couponsList) ? 
                couponsList.ToImmutableArray() : ImmutableArray<Coupon>.Empty;
            var customer = customerDict.GetValueOrDefault(dataModel.CustomerId);
            if (customer == null) {
                throw new CaasDomainMappingException($"Failed to find customer {dataModel.CustomerId} for order {dataModel.Id}");
            }
            domainModels.Add(new Order() {
                Id = dataModel.Id,
                Customer = customer,
                ShopId = dataModel.ShopId,
                Items = orderItems,
                OrderDate = dataModel.OrderDate,
                OrderNumber = dataModel.OrderNumber,
                Coupons = coupons,
                OrderDiscounts = orderDiscounts,
                Address = new Address() {
                    Street = dataModel.AddressStreet,
                    City = dataModel.AddressCity,
                    State = dataModel.AddressState,
                    Country = dataModel.AddressCountry,
                    ZipCode = dataModel.AddressZipCode
                },
                ConcurrencyToken = dataModel.GetConcurrencyToken()
            });
        }
        return domainModels;
    }
}