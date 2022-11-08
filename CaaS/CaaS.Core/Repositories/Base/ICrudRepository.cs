using CaaS.Core.Entities.Base;

namespace CaaS.Core.Repositories.Base;

public interface ICrudRepository<T> : ICrudReadRepository<T>, ICurdWriteRepository<T> where T : IEntityBase { }