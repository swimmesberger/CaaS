using CaaS.Core.CustomerAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.CustomerData;

public class CustomerRepository : CrudRepository<CustomerDataModel, Customer>, ICustomerRepository {
    public CustomerRepository(IDao<CustomerDataModel> dao) : 
            base(dao, new CustomerDomainModelConverter()) { }
}

internal class CustomerDomainModelConverter : IDomainModelConverter<CustomerDataModel, Customer> {
    public IEnumerable<OrderParameter>? DefaultOrderParameters { get; } = OrderParameter.From(nameof(CustomerDataModel.Name));
    
    public ValueTask<Customer> ConvertToDomain(CustomerDataModel dataModel, CancellationToken cancellationToken) {
        return new ValueTask<Customer>(new Customer() {
            Id = dataModel.Id,
            Name = dataModel.Name,
            ShopId = dataModel.ShopId,
            EMail = dataModel.EMail,
            CreditCardNumber = dataModel.CreditCardNumber,
            ConcurrencyToken = dataModel.GetConcurrencyToken()
        });
    }
    
    public async Task<IReadOnlyList<Customer>> ConvertToDomain(IAsyncEnumerable<CustomerDataModel> dataModels, CancellationToken cancellationToken = default) {
        return await dataModels.SelectAwaitWithCancellation(ConvertToDomain).ToListAsync(cancellationToken);
    }
    
    public CustomerDataModel ApplyDomainModel(CustomerDataModel dataModel, Customer domainModel) {
        return dataModel with {
            Id = domainModel.Id,
            Name = domainModel.Name,
            ShopId = domainModel.ShopId,
            EMail = domainModel.EMail,
            CreditCardNumber = domainModel.CreditCardNumber
        };
    }

    public Customer ApplyDataModel(Customer domainModel, CustomerDataModel dataModel) {
        return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
    }

    public CustomerDataModel ConvertFromDomain(Customer domainModel) {
        return new CustomerDataModel() {
            Id = domainModel.Id,
            Name = domainModel.Name,
            ShopId = domainModel.ShopId,
            EMail = domainModel.EMail,
            CreditCardNumber = domainModel.CreditCardNumber,
            RowVersion = domainModel.GetRowVersion()
        };
    }
}