using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DomainDataRepository.Services;
using EFDDD.DomainDataRepository;
using EFDDD.DomainModel;

namespace Api.Controllers
{
    [RoutePrefix("api/homeOwnership")]
    public class HomeOwnershipController : ApiController
    {
        private readonly DbAllRepositoryGroup _repositories;

        public HomeOwnershipController(DbAllRepositoryGroup repositories)
        {
            _repositories = repositories;
        }

        [HttpPost, Route("buy")]
        public async Task<IHttpActionResult> BuyHome([FromBody]BuyHomeRequest buyHome)
        {
            var service = new HomeOwnershipService(_repositories.Homes, _repositories.HomeOwners);

            await service.BuyHome(buyHome.HomeOwner, buyHome.Home);

            return Ok();
        }

        [HttpGet, Route("owners")]
        public async Task<IHttpActionResult> HomeOwners()
        {
            return Ok(_repositories.HomeOwners.All().ToList());
        }

        public class BuyHomeRequest
        {
            public HomeOwner HomeOwner { get; set; }
            public Home Home { get; set; }
        }
    }
}
