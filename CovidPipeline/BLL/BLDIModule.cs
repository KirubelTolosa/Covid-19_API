using Autofac;
using CovidPipeline.BAL;
using CovidPipeline.Covid_API.BAL;
using CovidPipeline.Covid_API.DAL;

namespace CovidPipeline.BAL
{
    public static class BLDIModule
    {
        public static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<CovidBLService>().As<ICovidBLService>();
            DALDIModule.RegisterServices(builder);            
        }
    }
}
