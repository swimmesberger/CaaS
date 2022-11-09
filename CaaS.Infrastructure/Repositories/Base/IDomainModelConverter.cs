namespace CaaS.Infrastructure.Repositories.Base; 

public interface IDomainModelConverter<TData, TDomain> : IDomainReadModelConverter<TData, TDomain> {
    TData ApplyDomainModel(TData dataModel, TDomain domainModel);

    TData ConvertFromDomain(TDomain domainModel);
}