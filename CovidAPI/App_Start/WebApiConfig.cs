using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CovidAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("CovidNationalCount", "api/covid/{metrics}/{NationalCount}", defaults: new { controller = "Covid", metrics = RouteParameter.Optional, NationalCount = RouteParameter.Optional});
            config.Routes.MapHttpRoute("CovidGlobalCount", "api/covid/{metrics}/GlobalCount", defaults: new { controller = "Covid", metrics = RouteParameter.Optional, GlobalCount = RouteParameter.Optional });
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
           
        }
    }
}
