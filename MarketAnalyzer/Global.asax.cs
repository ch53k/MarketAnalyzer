using System.Data.Entity;
using System.Web.Http;
using MarketAnalyzer.Model;
using MarketAnalyzer.Shared.DepedencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MarketAnalyzer
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var confiuration = Startup.ConfigureOptions();
            var serviceProvider = Startup.ConfigureServices(confiuration);

            GlobalConfiguration.Configure(config =>
            {
                config.MapHttpAttributeRoutes();

                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                var jsonSettings = config.Formatters.JsonFormatter.SerializerSettings;
                jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); //Use camel case.
                jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; //Prevent self reference loop issues when serializing entity objects.

                config.DependencyResolver = new DependencyResolver(serviceProvider);
            });

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AnalyzerDbContext, Migrations.Configuration>());
        }
    }
}
