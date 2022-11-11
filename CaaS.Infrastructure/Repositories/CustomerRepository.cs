using CaaS.Core.Entities;
using CaaS.Core.Repositories;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories.Base;

namespace CaaS.Infrastructure.Repositories;

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
        return await dataModels.SelectAwait(m => ConvertToDomain(m, cancellationToken)).ToListAsync(cancellationToken);
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