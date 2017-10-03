using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MarketAnalyzer.Modules.App.Repository;
using MarketAnalyzer.Shared;

namespace MarketAnalyzer.Modules.App.ApiControllers
{
    [RoutePrefix("api/stocks")]
    [AllowAnonymous]
    public class StocksController : AnalyzerControllerBase
    {

        public StocksController(StockRepository repository) : base(repository)
        {
        }
        [Route("tickers")]
        public async Task<IEnumerable<string>> GetTickers()
        {
            return await Repository.GetTickersAsync();
        }

        [Route("chart")]
        public async Task<StockWithChartVm> GetStockChart([FromUri] string ticker, [FromUri] string start, [FromUri] string end)
        {
            if (string.IsNullOrWhiteSpace(ticker) || string.IsNullOrWhiteSpace(start) || string.IsNullOrWhiteSpace(end))
            {
                return null;
            }

            ticker = ticker.ToUpperInvariant();
            return await Repository.GetChartAsync(ticker, start, end);
        }

        [Route("chart")]
        public async Task<StockWithChartVm> GetStockChart([FromUri] string ticker, [FromUri] int take)
        {
            if (string.IsNullOrWhiteSpace(ticker) || take < 0)
            {
                return null;
            }

            ticker = ticker.ToUpperInvariant();
            return await Repository.GetChartAsync(ticker, take);

        }

        [AllowAnonymous]
        public async Task<HttpResponseMessage> PostLoadStock([FromUri] string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Ticker must have a value.");
            }

            ticker = ticker.ToUpperInvariant();
            var result = await Repository.LoadStockAsync(ticker);
            return result.Succeeded ? Request.CreateResponse() : Request.CreateErrorResponse(HttpStatusCode.BadRequest, result.Error);
        }
    }

    
}