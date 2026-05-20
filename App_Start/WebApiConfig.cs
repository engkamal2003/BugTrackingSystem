using Newtonsoft.Json.Serialization;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BugTrackingSystem
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*"
            );
            config.EnableCors(cors);

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
