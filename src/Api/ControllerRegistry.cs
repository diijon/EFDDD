using System.Web.Http;
using System.Web.Http.Controllers;
using StructureMap.Configuration.DSL;
using Api.Controllers;
using EFDDD.DataModel.EF;
using EFDDD.DomainDataMapper;
using EFDDD.DomainDataRepository;
using EFDDD.DomainModel;

namespace Api
{
    public class ControllerRegistry : Registry
    {
        public ControllerRegistry(string connectionString)
        {
            Scan(p =>
            {
                p.AddAllTypesOf<IHttpController>();
            });

            MappingConfiguration.Configure();

            For<IMappingWorker>().Use(new MappingWorker(AutoMapper.Mapper.Engine));
            For<IContext>().Use<Context>()
                .Ctor<string>("nameOrConnectionString").Is(connectionString);
            For<IDbAllRepositoryGroup>().Use<DbAllRepositoryGroup>();
        }
    }
}