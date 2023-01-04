using SwKo.Pay.Api;
using SwKo.Pay.Api.Exceptions;
using SwKo.Pay.Models;

namespace SwKo.Pay; 

public class PaymentServiceImpl : IPaymentService {
    private static readonly IDictionary<string, CreditCard> CreditCardsDict = new Dictionary<string, CreditCard>();
    private static readonly IDictionary<string, Customer> CustomersDict = new Dictionary<string, Customer>();
    private static readonly IDictionary<Guid, Payment> PaymentsDict = new Dictionary<Guid, Payment>();

    static PaymentServiceImpl() {
        //Invalid card
        CreditCardsDict.Add("9999999999999999", new CreditCard(){CreditCardNumber = "9999999999999999", Active = true, Credit = (decimal)500.10});
        
        //Valid card but inactive (e.g. because lost or expired)
        CreditCardsDict.Add("4594233824721535", new CreditCard(){CreditCardNumber = "4594233824721535", Active = false, Credit = (decimal)487.94});
        
        //Valid, active card with little credit
        CreditCardsDict.Add("5349801875979163", new CreditCard(){CreditCardNumber = "5349801875979163", Active = true, Credit = (decimal)2.20});
        
        //Valid, active card with large credit
        CreditCardsDict.Add("4405100664466647", new CreditCard(){CreditCardNumber = "4405100664466647", Active = true, Credit = (decimal)10000.00});
        
        CustomersDict.Add("testcustomer@test.com", new Customer() {
            Id = Guid.Parse("5D5EA856-00B5-4F1F-BF2D-2AD81311B865"),
            Email = "gschiefersten0@scientificamerican.com",
            FirstName = "Gennie",
            LastName = "Schiefersten",
            CreditCardNumber = "4405100664466647",
            Metadata = new Dictionary<string, string> {
                {"CaasCustomerId", "709262d7-3b8c-4b90-b005-a698599c82b0"}
            }
        });
    }
    
    public Task<Guid> RegisterPayment(Customer customer, decimal amount, Currency currency) {
        CheckPreConditions(customer.CreditCardNumber, amount, currency);

        if (!CustomersDict.ContainsKey(customer.Email)) {
            CustomersDict.Add(customer.Email, customer);
        }

        Payment payment = new Payment {
            Id = Guid.NewGuid(),
            Customer = customer,
            Amount = amount,
            Currency = currency,
            Status = PaymentStatus.Pending
        };

        PaymentsDict.Add(payment.Id, payment);
        
        Task.Delay(2000);
        return Task.FromResult(payment.Id);
    }
    
    public Task ConfirmPayment(Guid paymentId) {
        if (!PaymentsDict.TryGetValue(paymentId, out var payment)) {
            throw new SwKoPayPaymentNotFoundException($"Payment {paymentId} not found");
        }

        return Task.FromResult(payment.Status = PaymentStatus.Confirmed);
    }
    
    public Task RefundPayment(Guid paymentId) {
        if (!PaymentsDict.TryGetValue(paymentId, out var payment)) {
            throw new SwKoPayPaymentNotFoundException($"Payment {paymentId} not found");
        }

        if (payment.Status.Equals(PaymentStatus.Confirmed)) {
            throw new SwKoPayCannotRefundConfirmedPayment($"{paymentId} cannot refund a payment that was already confirmed");
        }

        PaymentsDict.Remove(paymentId);
        return Task.CompletedTask;
    }

    private void CheckPreConditions(string creditCardNumber, decimal amount, Currency currency) {
        if (!CreditCardValidityChecker.IsValid(creditCardNumber)) {
            throw new SwKoPayCreditCardInvalidException($"Credit card {creditCardNumber} is invalid");
        }

        if (!CreditCardsDict.TryGetValue(creditCardNumber, out var creditCard)) {
            throw new SwKoPayCreditCardInvalidException($"Credit card {creditCardNumber} is not supported by SwRkPay");
        }

        if (creditCard.Active.Equals(false)) {
            throw new SwKoPayCardInactiveException($"Credit card {creditCardNumber} is not active anymore");
        }

        if (creditCard.Credit < amount) {
            throw new SwKoPayInsufficientCreditException($"The amount of '{amount}' is not covered by credit card {creditCardNumber}");
        }
    }
}
