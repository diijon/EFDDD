using EFDDD.Core;
using EFDDD.DataModel.EF;
using EFDDD.DomainDataMapper;

namespace EFDDD.DomainDataRepository.Contracts
{
    public class DbRepositoryGroup : RepositoryGroup
    {
        public readonly IContext Context;
        public readonly IMappingWorker Mapper;

        protected DbRepositoryGroup(IMappingWorker mapper, IContext context)
        {
            Context = context;
            Mapper = mapper;
        }

        internal GenericDbRepository<TDomainEntity, TDataEntity> GetGenericRepository<TDomainEntity, TDataEntity>()
            where TDomainEntity : class, IDomainEntity
            where TDataEntity : class, IDataEntity
        {
            return Find<GenericDbRepository<TDomainEntity, TDataEntity>>() as GenericDbRepository<TDomainEntity, TDataEntity>;
        }

        public override void Save()
        {
            Context.SaveChanges();
        }
    }
}