using CaaS.Core.Entities;
using CaaS.Core.Exceptions;
using CaaS.Core.Repositories;
using CaaS.Core.Request;

namespace CaaS.Core.Services; 

public class OrderService : IOrderService {
    private readonly IOrderRepository _orderRepository;
    private readonly ITenantIdAccessor _tenantIdAccessor;
    
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository, ITenantIdAccessor tenantIdAccessor, 
        ICustomerRepository customerRepository, IProductRepository productRepository) {
        _orderRepository = orderRepository;
        _tenantIdAccessor = tenantIdAccessor;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public async Task<Order?> FindOrderById(Guid orderId, CancellationToken cancellationToken = default) {
        var order = await _orderRepository.FindByIdAsync(orderId, cancellationToken);
        return order ?? null;
    }
    
    public async Task<Order> CreateOrder(Guid customerId ,CancellationToken cancellationToken = default) {
        var shortenedCustomerId = customerId.ToString()[..8];
        var customer = await _customerRepository.FindByIdAsync(customerId);
        if (customer == null) {
            throw new CaasModelNotFoundException();
        }
        
        var rand = new Random();
        var order = new Order() {
            ShopId = _tenantIdAccessor.GetTenantGuid(),
            Customer = customer,
            OrderDate = DateTimeOffset.Now
        };
        return await _orderRepository.AddAsync(order, cancellationToken);
    }
    
    public async Task<Order> AddProductToOrder(Guid orderId, Guid productId, int productQuantity, CancellationToken cancellationToken = default) {
        if (productQuantity <= 0) {
            throw new ArgumentException("Invalid product quantity", nameof(productQuantity));
        }

        var order = await _orderRepository.FindByIdAsync(orderId, cancellationToken);
        if (order == null) {
            throw new CaasModelNotFoundException();
        }
        var originalOrder = order;

        var productItemIdx = order.Items.FindIndex(p => p.Product.Id == productId);
        if (productItemIdx != -1) { //Product already exists in order... will this case ever happen?
            var productItem = order.Items[productItemIdx];
            productItem = productItem with {
                Amount = productItem.Amount + productQuantity
            };

            order = order with {
                Items = order.Items.SetItem(productItemIdx, productItem),
            };
        } else {        //add new item to order
            var product = await _productRepository.FindByIdAsync(productId, cancellationToken);
            if (product == null) {
                throw new CaasModelNotFoundException();
            }

            var productItem = new OrderItem {
                Product = new Product() { Id = productId },
                ShopId = _tenantIdAccessor.GetTenantGuid(),
                OrderId = orderId,
                Amount = productQuantity,
                //OrderItemDiscounts = null,
                PricePerPiece = product.Price,
            };

            order = order with {
                Items = order.Items.Add(productItem)
            };
        }
        return await _orderRepository.UpdateAsync(originalOrder, order, cancellationToken);
    }
    public Task<Order> CreateOrderFromCart(Guid cartId, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
}