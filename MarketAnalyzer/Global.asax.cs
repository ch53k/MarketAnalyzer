using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using MarketAnalyzer.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MarketAnalyzer
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
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
            });

            //GlobalConfiguration.Configure(WebApiConfig.Register);
            Globals.StockQuoteKey = ConfigurationManager.ConnectionStrings["StockQuotes"].ConnectionString;

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AnalyzerDbContext, Migrations.Configuration>());
        }
    }
}
