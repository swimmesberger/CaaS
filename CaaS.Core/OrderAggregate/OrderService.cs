using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.CartAggregate;
using CaaS.Core.CouponAggregate;
using SwKo.Pay.Api;
using SwKo.Pay.Api.Exceptions;
using Customer = CaaS.Core.CustomerAggregate.Customer;
using CustomerPay = SwKo.Pay.Api.Customer;

namespace CaaS.Core.OrderAggregate; 

public class OrderService : IOrderService {
    private readonly IOrderRepository _orderRepository;
    private readonly ICartService _cartService;
    private readonly ICouponRepository _couponRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly ITenantIdAccessor _tenantIdAccessor;
    private readonly IPaymentService _paymentService;

    public OrderService(IOrderRepository orderRepository, ICartService cartService,
        ICouponRepository couponRepository, ITenantIdAccessor tenantIdAccessor, IUnitOfWorkManager unitOfWorkManager,
        IPaymentService paymentService) {
        _orderRepository = orderRepository;
        _cartService = cartService;
        _couponRepository = couponRepository;
        _tenantIdAccessor = tenantIdAccessor;
        _unitOfWorkManager = unitOfWorkManager;
        _paymentService = paymentService;
    }

    public async Task<Order?> FindByIdAsync(Guid orderId, CancellationToken cancellationToken = default) {
        return await _orderRepository.FindByIdAsync(orderId, cancellationToken);
    }

    public async Task<Order> CreateOrderFromCartAsync(Guid cartId, Address billingAddress, Customer? userCustomer = null, 
        CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var cart = await _cartService.SetCustomerAsync(cartId, userCustomer, cancellationToken);
        if (cart.Items.Count <= 0) {
            throw new CaasValidationException("Cart must have at least a single item");
        }
        var customer = cart.Customer!;
        Order order;
        var chargeId = Guid.Empty;
        try {
            var chargeMetadata = new Dictionary<string, string>() {
                { "CaasCustomerId", customer.Id.ToString() }
            };
            var seKoPayCustomer = new CustomerPay() {
                Email = customer.EMail,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                CreditCardNumber = customer.CreditCardNumber,
                Metadata = chargeMetadata
            };
            chargeId = await _paymentService.RegisterPayment(seKoPayCustomer, cart.TotalPrice, Currency.EUR);
            order = await CreateOrderFromCartImpl(cart, customer, billingAddress, cancellationToken);
            await uow.CompleteAsync(cancellationToken);
            await _paymentService.ConfirmPayment(chargeId);
        } catch (CaasDbException e) {
            await _paymentService.RefundPayment(chargeId);
            throw new CaasDbException("CaaS database transaction error. Payment was refunded", e);
        } catch (SwKoPayCardInactiveException ex) { //transaction not committed by default, if uow.CompleteAsync is not called
            throw new CaasPaymentCardInvalidException("Credit card is inactive. Order was not successful.", ex) {
                Type = "payment_inactive_card"
            };
        } catch (SwKoPayCreditCardInvalidException ex) {
            throw new CaasPaymentCardInvalidException("Credit card is invalid. Order was not successful.", ex) {
                Type = "payment_invalid_card"
            };
        } catch (SwKoPayInsufficientCreditException ex) {
            throw new CaasPaymentInsufficientCreditException($"{cart.TotalPrice} is not covered by provided credit card", ex) {
                Type = "payment_insufficient_credit"
            };
        } catch (SwKoPayPaymentNotFoundException ex) {
            throw new CaasPaymentException(ex.Message);
        } catch (SwKoPayCannotRefundConfirmedPayment ex) {
            throw new CaasPaymentException(ex.Message);
        }
        
        return order;
    }
    
    private async Task<Order> CreateOrderFromCartImpl(Cart cart, Customer customer, Address billingAddress, CancellationToken cancellationToken = default) {
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
        
        var order = new Order {
            Id = orderId,
            ShopId = _tenantIdAccessor.GetTenantGuid(),
            Customer = customer,
            Items = orderItems.ToImmutableArray(),
            Address = billingAddress,
            OrderDiscounts = cart.CartDiscounts.Select(d => d with { ParentId = orderId }).ToImmutableArray(),
            OrderDate = SystemClock.GetNow()
        };

        await _cartService.DeleteAsync(cart.Id, cancellationToken);
        var updatedCoupons = cart.Coupons.Select(c => c with { CartId = null, CustomerId = customer.Id, OrderId = orderId });
        order = await _orderRepository.AddAsync(order, billingAddress, cancellationToken);
        var orderNumber = await _orderRepository.FindOrderNumberById(orderId, cancellationToken);
        order = order with { OrderNumber = orderNumber };
        await _couponRepository.UpdateAsync(cart.Coupons, updatedCoupons, cancellationToken);
        var coupons = await _couponRepository.FindByOrderIdAsync(orderId, cancellationToken);
        order = order with { Coupons = coupons.ToImmutableArray() };
        return order;
    }
}