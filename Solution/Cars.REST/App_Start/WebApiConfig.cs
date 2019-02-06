using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Cars.REST
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //Register Mappers
            AutoMapperConfig.Register();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v1/{controller}/{agencynumber}/{resource}",
                defaults: new { controller = "agencies", agencynumber = RouteParameter.Optional, resource = RouteParameter.Optional }
            );
        }
    }
}
