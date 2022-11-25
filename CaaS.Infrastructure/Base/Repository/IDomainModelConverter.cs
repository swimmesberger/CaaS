namespace CaaS.Infrastructure.Base.Repository; 

public interface IDomainModelConverter<TData, TDomain> : IDomainReadModelConverter<TData, TDomain> {
    TData ApplyDomainModel(TData dataModel, TDomain domainModel);

    TDomain ApplyDataModel(TDomain domainModel, TData dataModel);

    TData ConvertFromDomain(TDomain domainModel);
}