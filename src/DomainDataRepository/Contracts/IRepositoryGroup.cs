using System;
using System.Collections.Generic;
using EFDDD.Core;

namespace EFDDD.DomainDataRepository.Contracts
{
    public interface IRepositoryGroup : IDisposable
    {
        IList<IDomainRepository> Repositories { get; }
        IDomainRepository Find<TRepositoryToMatch>() where TRepositoryToMatch : IDomainRepository;
        IDomainRepository FindByEntity<TEntity>() where TEntity : IDomainEntity;
        void Save();
    }
}