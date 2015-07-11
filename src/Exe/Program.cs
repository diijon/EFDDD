using System;
using CommandLine;
using EFDDD.DataModel.Migrations.Core.ConsoleRunner;
using EFDDD.DataModel.Migrations.Core.Contracts;
using System.Threading;

namespace EFDDD.Exe
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandLineArgs = new CommandLineOptions();
            if (!Parser.Default.ParseArguments(args, commandLineArgs))
            {
                Console.WriteLine("The CommandLine arguments are invalid. Please view the help text for each parameter.");
                return;
            }

            var container = BootStrap.Strap(commandLineArgs);

            var runner = container.GetInstance<IMigrationRunner>();
            runner.Run().Wait();
        }
    }
}
