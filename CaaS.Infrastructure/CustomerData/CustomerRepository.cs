using CaaS.Core.CustomerAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.CustomerData;

public class CustomerRepository : CrudRepository<CustomerDataModel, Customer>, ICustomerRepository {
    public CustomerRepository(IDao<CustomerDataModel> dao) : 
            base(dao, new CustomerDomainModelConverter()) { }
    
    public async Task<Customer?> FindByEmailAsync(string email, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindBy(StatementParameters.CreateWhere(nameof(CustomerDataModel.EMail), email), cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }
}

internal class CustomerDomainModelConverter : IDomainModelConverter<CustomerDataModel, Customer> {
    public OrderParameters DefaultOrderParameters { get; } = new(
        new OrderParameter[] {
            new(nameof(CustomerDataModel.FirstName)), 
            new(nameof(CustomerDataModel.LastName))
        }
    );

    public ValueTask<Customer> ConvertToDomain(CustomerDataModel dataModel, CancellationToken cancellationToken) {
        return new ValueTask<Customer>(new Customer() {
            Id = dataModel.Id,
            FirstName = dataModel.FirstName,
            LastName = dataModel.LastName,
            ShopId = dataModel.ShopId,
            EMail = dataModel.EMail,
            TelephoneNumber = dataModel.TelephoneNumber,
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
            FirstName = domainModel.FirstName,
            LastName = domainModel.LastName,
            ShopId = domainModel.ShopId,
            EMail = domainModel.EMail,
            TelephoneNumber = dataModel.TelephoneNumber,
            CreditCardNumber = domainModel.CreditCardNumber
        };
    }

    public Customer ApplyDataModel(Customer domainModel, CustomerDataModel dataModel) {
        return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
    }

    public CustomerDataModel ConvertFromDomain(Customer domainModel) {
        return new CustomerDataModel() {
            Id = domainModel.Id,
            FirstName = domainModel.FirstName,
            LastName = domainModel.LastName,
            ShopId = domainModel.ShopId,
            EMail = domainModel.EMail,
            TelephoneNumber = domainModel.TelephoneNumber,
            CreditCardNumber = domainModel.CreditCardNumber,
            RowVersion = domainModel.GetRowVersion()
        };
    }
}