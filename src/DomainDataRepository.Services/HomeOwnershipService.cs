using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFDDD;
using EFDDD.Core;
using EFDDD.DomainModel;
using EFDDD.DataModel.EF;
using EFDDD.DomainDataMapper;
using EFDDD.DomainDataRepository;
using EFDDD.DomainDataRepository.Contracts;
using EFDDD.DomainDataRepository.Filters;
using EFDDD.DomainDataRepository.Sorters;

namespace DomainDataRepository.Services
{
    public class HomeOwnershipService
    {
        private readonly Dictionary<Type, object> _repositories;

        public HomeOwnershipService(IDomainRepository<Home> homes, IDomainRepository<HomeOwner> homeOwners)
        {
            if (homes == null) { throw new ArgumentNullException("homes"); }
            if (homeOwners == null) { throw new ArgumentNullException("homeOwners"); }

            _repositories = new Dictionary<Type, object>
            {
                {typeof(Home), homes},
                {typeof(HomeOwner), homeOwners}
            };
        }

        private IDomainRepository<TDomainEntity> GetRepository<TDomainEntity>()
        {
            return _repositories[typeof(TDomainEntity)] as IDomainRepository<TDomainEntity>;
        }

        public async Task BuyHome(HomeOwner homeOwner, Home home)
        {
            if (homeOwner == null) { throw new ArgumentNullException("homeOwner"); }
            if (home == null) { throw new ArgumentNullException("home"); }

            homeOwner.Id = homeOwner.Id.SetToGuidIfNullOrEmpty(Guid.NewGuid());
            home.Id = home.Id.SetToGuidIfNullOrEmpty(Guid.NewGuid());
            home.HomeOwnerId = homeOwner.Id;

            GetRepository<Home>().Merge(home);
            GetRepository<HomeOwner>().Merge(homeOwner);
            GetRepository<Home>().Save();

            //TODO: Add Transaction to HomeSales repository
        }
    }
}
