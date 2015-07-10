using System;
using System.Collections.Generic;
using System.Linq;
using EFDDD.Core;

namespace EFDDD.DomainDataRepository.Contracts
{
    public class RepositoryGroup : IRepositoryGroup
    {
        bool disposed = false;

        private readonly IList<IDomainRepository> _repositories = new List<IDomainRepository>();
        public IList<IDomainRepository> Repositories
        {
            get { return _repositories; }
        }

        public IDomainRepository Find<TRepositoryToMatch>() where TRepositoryToMatch : IDomainRepository
        {
            var repository = Repositories.FirstOrDefault(x => x is TRepositoryToMatch);
            if (repository == null) { throw new IndexOutOfRangeException(typeof(TRepositoryToMatch).Name); }

            return repository;
        }

        public IDomainRepository FindByEntity<TEntity>() where TEntity : IDomainEntity
        {
            var repository = Repositories.FirstOrDefault(x => x is IDomainRepository<TEntity>);
            if (repository == null) { throw new IndexOutOfRangeException(typeof(TEntity).Name); }

            return repository;
        }

        public virtual void Save()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                for (var i = _repositories.Count() - 1; i >= 0; i--)
                {
                    _repositories.ElementAt(i).Dispose();
                }
            }

            disposed = true;
        }
    }
}