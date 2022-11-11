using CaaS.Core.Customer;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.Customer;

public class CustomerRepository : CrudRepository<CustomerDataModel, Core.Customer.Entities.Customer>, ICustomerRepository {
    public CustomerRepository(IDao<CustomerDataModel> dao) : 
            base(dao, new CustomerDomainModelConverter()) { }
}

internal class CustomerDomainModelConverter : IDomainModelConverter<CustomerDataModel, Core.Customer.Entities.Customer> {
    public IEnumerable<OrderParameter>? DefaultOrderParameters { get; } = OrderParameter.From(nameof(CustomerDataModel.Name));
    
    public ValueTask<Core.Customer.Entities.Customer> ConvertToDomain(CustomerDataModel dataModel, CancellationToken cancellationToken) {
        return new ValueTask<Core.Customer.Entities.Customer>(new Core.Customer.Entities.Customer() {
            Id = dataModel.Id,
            Name = dataModel.Name,
            ShopId = dataModel.ShopId,
            EMail = dataModel.EMail,
            CreditCardNumber = dataModel.CreditCardNumber,
            ConcurrencyToken = dataModel.GetConcurrencyToken()
        });
    }
    
    public async Task<IReadOnlyList<Core.Customer.Entities.Customer>> ConvertToDomain(IAsyncEnumerable<CustomerDataModel> dataModels, CancellationToken cancellationToken = default) {
        return await dataModels.SelectAwait(m => ConvertToDomain(m, cancellationToken)).ToListAsync(cancellationToken);
    }
    
    public CustomerDataModel ApplyDomainModel(CustomerDataModel dataModel, Core.Customer.Entities.Customer domainModel) {
        return dataModel with {
            Id = domainModel.Id,
            Name = domainModel.Name,
            ShopId = domainModel.ShopId,
            EMail = domainModel.EMail,
            CreditCardNumber = domainModel.CreditCardNumber
        };
    }
    
    public CustomerDataModel ConvertFromDomain(Core.Customer.Entities.Customer domainModel) {
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