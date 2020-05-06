using Autofac;
using CovidPipeline.BAL;
using CovidPipeline.Covid_API.BAL;
using CovidPipeline.Covid_API.DAL;
using CovidPipeline.DAL;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CovidPipeline
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        private static Autofac.IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Application>();
            builder.RegisterType<CovidBLService>().As<ICovidBLService>();
            builder.RegisterType<CovidDataRepository>().As<ICovidDataRepository>();
            return builder.Build();
        }
        public static void Main(string[] args)
        {
            if (args.Count() > 0 && args[0] == "Full")
                Console.WriteLine("Your console parameter is 'Full'");
            else
                CompositionRoot().Resolve<Application>().Run();
        }
    }
}
