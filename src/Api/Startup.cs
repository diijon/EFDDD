using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Web.Http;
using EFDDD.DataModel.EF;
using EFDDD.DomainDataMapper;
using EFDDD.DomainDataRepository;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using StructureMap;

[assembly: OwinStartup(typeof(Api.Startup))]

namespace Api
{
    public class Startup
    {
        public static void Configuration(IAppBuilder app, string connectionString)
        {
            HttpConfiguration config = new HttpConfiguration();

            var container = new Container(c => c.AddRegistry(new ControllerRegistry(connectionString)));
            config.DependencyResolver =
                new DependencyResolver(container);
            container.AssertConfigurationIsValid();

            config.MapHttpAttributeRoutes();

            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config); 
        }
    }
}
