using System.Web.Http;
using MarketAnalyzer.Model;
using MarketAnalyzer.Modules.App.Repository;

namespace MarketAnalyzer.Shared
{
    public class AnalyzerControllerBase : ApiController
    {
        protected readonly StockRepository Repository;

        public AnalyzerControllerBase(StockRepository repository)
        {
            Repository = repository;
        }
    }
}