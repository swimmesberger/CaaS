namespace SwKo.Pay.Api; 

public interface IPaymentService {
    
    /// <summary>
    /// Registers a payment that still needs to be confirmed to be charged
    /// Throws
    ///     SwKoPayCreditCardInvalidException: if the card is not supported by SwKoPay
    ///     SwKoPayCreditCardInactiveException: if the card is not active anymore (e.g. expired or reported as lost)
    ///     SwKoPayInsufficientCreditExcpetion: if the remaining credit is not sufficient for the payable amount
    /// </summary>
    /// <param name="customer">A SwKoPay Customer</param>
    /// <param name="amount">Amount of payment</param>
    /// <param name="currency">Currency of payment</param>
    /// <returns>A paymentId</returns>
    Task<Guid> RegisterPayment(Customer customer, decimal amount, Currency currency);
    
    /// <summary>
    /// Confirms a payment that was previously registered.
    /// Only after confirming a payment the card will be charged-
    /// </summary>
    /// <param name="paymentId">The payment that is confirmed</param>
    /// <returns></returns>
    Task ConfirmPayment(Guid paymentId);
    
    /// <summary>
    /// Refunds a pending payment that was not yet confirmed
    /// Throws
    ///     SwKoPayPaymentNotFoundException: if payment is not found in system
    ///     SwKoPayCannotRefundConfirmedPayment: if an already confirmed payment is tried to be refunded (only pending payments can be refunded)
    /// </summary>
    /// <param name="paymentId">The paymentId that should be refunded</param>
    /// <returns></returns>
    Task RefundPayment(Guid paymentId);

}
