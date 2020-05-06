using Autofac;
using CovidPipeline.DAL;

namespace CovidPipeline.Covid_API.DAL
{
    public static class DALDIModule
    {
        public static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<CovidDataRepository>().As<ICovidDataRepository>();            
        }
    }
}
