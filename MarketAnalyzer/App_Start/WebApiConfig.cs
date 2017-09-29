using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MarketAnalyzer
{
    public class WebApiHttpConfiguration : HttpConfiguration
    {
        public WebApiHttpConfiguration()
        {
            ConfigureRoutes();
            ConfigureJsonSerialization();

            //Filters.Add(new WebApiExceptionFilterAttribute());
        }

        private void ConfigureRoutes()
        {
            this.MapHttpAttributeRoutes();
        }

        private void ConfigureJsonSerialization()
        {
            var jsonSettings = Formatters.JsonFormatter.SerializerSettings;
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); //Use camel case.
            jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; //Prevent self reference loop issues when serializing entity objects.
        }
    }
}