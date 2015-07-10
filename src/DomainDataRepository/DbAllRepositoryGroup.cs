using System.Text;
using System.Threading.Tasks;
using EFDDD.DataModel.EF;
using EFDDD.DomainDataMapper;
using EFDDD.DomainDataRepository.Contracts;

namespace EFDDD.DomainDataRepository
{
    public interface IDbAllRepositoryGroup : IRepositoryGroup
    {
        IDomainRepository<DomainModel.Home> Homes { get; }
        IDomainRepository<DomainModel.HomeOwner> HomeOwners { get; }
    }

    public class DbAllRepositoryGroup : DbRepositoryGroup, IDbAllRepositoryGroup
    {
        public DbAllRepositoryGroup(IMappingWorker mapper, IContext context) : base(mapper, context)
        {
            Repositories.Add(new GenericDbRepository<DomainModel.Home, DataModel.Home>(mapper, context));
            Repositories.Add(new GenericDbRepository<DomainModel.HomeOwner, DataModel.HomeOwner>(mapper, context));
        }

        public IDomainRepository<DomainModel.Home> Homes
        {
            get { return GetGenericRepository<DomainModel.Home, DataModel.Home>(); }
        }
        public IDomainRepository<DomainModel.HomeOwner> HomeOwners
        {
            get { return GetGenericRepository<DomainModel.HomeOwner, DataModel.HomeOwner>(); }
        }
    }
}
