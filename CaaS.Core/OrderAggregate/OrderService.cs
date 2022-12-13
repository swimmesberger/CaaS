using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.CartAggregate;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using SwKo.Pay.Api;
using SwKo.Pay.Api.Exceptions;
using Customer = CaaS.Core.CustomerAggregate.Customer;
using CustomerPay = SwKo.Pay.Api.Customer;

namespace CaaS.Core.OrderAggregate; 

public class OrderService : IOrderService {
    private readonly IOrderRepository _orderRepository;
    private readonly ICartService _cartService;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly ITenantIdAccessor _tenantIdAccessor;
    private readonly IPaymentService _paymentService;

    public OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepository, ICartService cartService,
        ICouponRepository couponRepository, ITenantIdAccessor tenantIdAccessor, IUnitOfWorkManager unitOfWorkManager,
        IPaymentService paymentService) {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _cartService = cartService;
        _couponRepository = couponRepository;
        _tenantIdAccessor = tenantIdAccessor;
        _unitOfWorkManager = unitOfWorkManager;
        _paymentService = paymentService;
    }

    public async Task<Order?> FindOrderById(Guid orderId, CancellationToken cancellationToken = default) {
        var order = await _orderRepository.FindByIdAsync(orderId, cancellationToken);
        return order ?? null;
    }
    
    public async Task<Order> CreateOrder(Guid customerId, Address billingAddress, CancellationToken cancellationToken = default) {
        var customer = await _customerRepository.FindByIdAsync(customerId, cancellationToken);
        if (customer == null) {
            throw new CaasItemNotFoundException();
        }
        
        var order = new Order() {
            ShopId = _tenantIdAccessor.GetTenantGuid(),
            Customer = customer,
            OrderDate = SystemClock.GetNow()
        };
        return await _orderRepository.AddAsync(order, billingAddress, cancellationToken);
    }

    public async Task<Order> CreateOrderFromCart(Guid cartId, Address billingAddress, CancellationToken cancellationToken = default) {
        var cart = await _cartService.GetCartById(cartId, cancellationToken);
        if (cart == null) {
            throw new CaasItemNotFoundException();
        }

        if (cart.Customer == null) {
            throw new CaasValidationException("cart needs a customer");
        }
        
        return await CreateOrderFromCartImpl(cart, cart.Customer, billingAddress, cancellationToken);
    }

    public async Task<Order> CreateOrderFromCart(Guid cartId, Customer customer, Address billingAddress, CancellationToken cancellationToken = default) {
        var cart = await _cartService.GetCartById(cartId, cancellationToken);
        if (cart == null) {
            throw new CaasItemNotFoundException();
        }

        if (cart.Customer != null) {
            throw new CaasValidationException("cart customer must be null if customer is provided");
        }
        
        await _customerRepository.AddAsync(customer, cancellationToken);
        return await CreateOrderFromCartImpl(cart, customer, billingAddress, cancellationToken);
    }
    
    private async Task<Order> CreateOrderFromCartImpl(Cart cart, Customer customer, Address billingAddress, CancellationToken cancellationToken = default) {
        Order order;
        var chargeId = Guid.Empty;
        await using var uow = _unitOfWorkManager.Begin();
        
        try {
            Dictionary<string, string> metadata = new Dictionary<string, string>() {
                { "CaasCustomerId", customer.Id.ToString() }
            };

            CustomerPay seKoPayCustomer = new CustomerPay() {
                Email = customer.EMail,
                Name = customer.Name,
                CreditCardNumber = customer.CreditCardNumber,
                Metadata = metadata
            };

            chargeId = await _paymentService.RegisterPayment(seKoPayCustomer, cart.TotalPrice, Currency.EUR);

            var orderId = Guid.NewGuid();

            var orderItems = new List<OrderItem>();
            foreach (var item in cart.Items) {
                var orderItem = new OrderItem {
                    Id = Guid.NewGuid(),
                    Product = item.Product,
                    ShopId = item.ShopId,
                    OrderId = orderId,
                    Amount = item.Amount,
                    OrderItemDiscounts = item.CartItemDiscounts.Select(d => d with { ParentId = orderId }).ToImmutableArray(),
                    PricePerPiece = item.Product.Price
                };
                orderItems.Add(orderItem);
            }

            
            order = new Order {
                Id = orderId,
                ShopId = _tenantIdAccessor.GetTenantGuid(),
                Customer = customer,
                Items = orderItems.ToImmutableArray(),
                Address = billingAddress,
                OrderDiscounts = cart.CartDiscounts.Select(d => d with { ParentId = orderId }).ToImmutableArray(),
                OrderDate = SystemClock.GetNow()
            };

            await _cartService.DeleteCart(cart.Id, cancellationToken);
            var updatedCoupons = cart.Coupons.Select(c => c with { CartId = null, CustomerId = customer.Id, OrderId = orderId });
            await _couponRepository.UpdateAsync(cart.Coupons, updatedCoupons, cancellationToken);
            
            order = await _orderRepository.AddAsync(order, billingAddress, cancellationToken);
            await uow.CompleteAsync(cancellationToken);
            
            await _paymentService.ConfirmPayment(chargeId);
        } catch (CaasDbException e) {
            await _paymentService.RefundPayment(chargeId);
            throw new CaasDbException("CaaS transaction error", e);
        } catch (SwKoPayPaymentNotFoundException e) {
            throw new SwKoPayPaymentNotFoundException("SwKo service could not confirm payment", e);
        }
        
        return order;
    }
}