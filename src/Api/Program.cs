using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EFDDD.DataModel.EF;
using EFDDD.DomainDataMapper;
using EFDDD.DomainDataRepository;
using EFDDD.DomainModel;
using Microsoft.Owin.Hosting;

namespace Api
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseAddress = "http://localhost:8067/";

            var connectionString = Helpers.GetRandomConnectionString();
            var dbContext = new Context(connectionString);

            Helpers.SetInitializer(new ContextInitializer(context =>
            {
                using (var repositories = new DbAllRepositoryGroup(new MappingWorker(AutoMapper.Mapper.Engine), context))
                {
                    //TODO: Seed for api testing
                }
            }));

            // Start OWIN host 
            using (WebApp.Start(baseAddress, app =>
            {
                Startup.Configuration(app, connectionString);
            }))
            {
                Console.ReadLine();
                Console.WriteLine();
                dbContext.Database.Delete();
            }
        }
    }
}
