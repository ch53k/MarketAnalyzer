using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MarketAnalyzer.Shared;

namespace MarketAnalyzer.Modules.App.ApiControllers
{
    [RoutePrefix("api/stocks")]
    [AllowAnonymous]
    public class StocksController : AnalyzerControllerBase
    {
        [Route("tickers")]
        public async Task<IEnumerable<string>> GetTickers()
        {
             return await Dbcontext.Stocks.Select(s => s.Id).ToListAsync();
        }
        
        public async Task<StockVm> GetStock([FromUri] string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                return null;
            }

            ticker = ticker.ToUpperInvariant();
            var stockVm = await Dbcontext.Stocks.Select(s => new StockVm() {Ticker = s.Id, MinDate = s.MinDate, MaxDate = s.MaxDate}).FirstOrDefaultAsync(s=>s.Ticker == ticker);

            stockVm.AllTimeHigh = await Dbcontext.StockQuotes.Where(s=>s.Ticker == ticker).MaxAsync(s => s.High);
            stockVm.AllTimeLow = await Dbcontext.StockQuotes.Where(s=>s.Ticker == ticker).MinAsync(s => s.Low);

            var fiftyTwoWeekAgo = DateTime.Today.AddYears(-1);
            stockVm.FiftyTwoWeekHigh = await Dbcontext.StockQuotes.Where(s=>s.Ticker == ticker && s.Date>=fiftyTwoWeekAgo).MaxAsync(s => s.High);
            stockVm.FiftyTwoWeekLow = await Dbcontext.StockQuotes.Where(s=>s.Ticker == ticker && s.Date>=fiftyTwoWeekAgo).MinAsync(s => s.Low);

            return stockVm;
        }
        
        [Route("chart")]
        public async Task<IEnumerable<StockQuoteChartVm>> GetStockChart([FromUri] string ticker, [FromUri] string start, [FromUri] string end)
        {
            if (string.IsNullOrWhiteSpace(ticker) || string.IsNullOrWhiteSpace(start) || string.IsNullOrWhiteSpace(end))
            {
                return null;
            }

            var startDate = new DateTime(int.Parse(start.Substring(0, 4)), int.Parse(start.Substring(4, 2)), int.Parse(start.Substring(6, 2)));
            var endDate = new DateTime(int.Parse(end.Substring(0, 4)), int.Parse(end.Substring(4, 2)), int.Parse(end.Substring(6, 2)));

            ticker = ticker.ToUpperInvariant();
            var quotes = await Dbcontext.StockQuotes.Where(s=>s.Ticker==ticker && s.Date>=startDate && s.Date<=endDate).Select(s => new {s.Date, s.Close}).ToListAsync();

            return quotes.Select(s => new StockQuoteChartVm() {Date = s.Date.ToShortDateString(), Close = s.Close});
        }

        [AllowAnonymous]
        public async Task<HttpResponseMessage> PostLoadStock([FromUri] string ticker)
        {
            var result = await StockLoader.Load(Dbcontext, ticker);
            return result.Succeeded ? Request.CreateResponse() : Request.CreateErrorResponse(HttpStatusCode.BadRequest, result.Error);
        }
    }

    public class StockQuoteChartVm
    {
        public string Date { get; set; }
        public decimal Close { get; set; }
    }

    public class StockVm
    {
        public string Ticker { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public decimal AllTimeHigh { get; set; }
        public decimal AllTimeLow { get; set; }
        public decimal FiftyTwoWeekHigh { get; set; }
        public decimal FiftyTwoWeekLow { get; set; }
    }
}