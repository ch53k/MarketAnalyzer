using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MarketAnalyzer.Model;
using MarketAnalyzer.Shared;
using MarketAnalyzer.Shared.Stocks;

namespace MarketAnalyzer.Modules.App.Repository
{
    public class StockRepository :IDisposable
    {
        private readonly IStockProcessor _stockLoader;
        protected readonly AnalyzerDbContext DbContext;
        public StockRepository(AnalyzerDbContext dbContext, IStockProcessor stockLoader)
        {
            DbContext = dbContext;
            _stockLoader = stockLoader;
        }

        public void Dispose()
        {
            DbContext?.Dispose();
        }

        public async Task<IEnumerable<string>> GetTickersAsync()
        {
            return await DbContext.Stocks.Select(s => s.Id).ToListAsync();
        }

        public async Task<StockWithChartVm> GetChartAsync(string ticker, string start, string end)
        {
            ticker = ticker.ToUpperInvariant();
            if (!(await _stockLoader.LoadCompactAsync(ticker)).Succeeded)
            {
                return null;
            }

            var startDate = new DateTime(int.Parse(start.Substring(0, 4)), int.Parse(start.Substring(4, 2)),int.Parse(start.Substring(6, 2)));
            var endDate = new DateTime(int.Parse(end.Substring(0, 4)), int.Parse(end.Substring(4, 2)),int.Parse(end.Substring(6, 2)));

            var quotes = await DbContext.StockQuotes
                .Where(s => s.Ticker == ticker && s.Date >= startDate && s.Date <= endDate)
                .Select(s => new { s.Date, s.Close, s.Open, s.High, s.Low }).OrderBy(d => d.Date).ToListAsync();

            if (!quotes.Any())
            {
                return null;
            }

            var stockQuoteVm = await GetStockInternalAsync(ticker);
            stockQuoteVm.ChartQuotes = quotes.Select(s => new StockQuoteChartVm()
            {
                Date = s.Date.ToShortDateString(),
                Close = s.Close,
                Open = s.Open,
                Low = s.Low,
                High = s.High
            });
            return stockQuoteVm;
        }

        public async Task<StockWithChartVm> GetChartAsync(string ticker, int take)
        {
            ticker = ticker.ToUpperInvariant();
            if (!(await _stockLoader.LoadCompactAsync(ticker)).Succeeded)
            {
                return null;
            }

            var quotes = await DbContext.StockQuotes.Where(s => s.Ticker == ticker).OrderByDescending(d => d.Date)
                .Take(take).Select(s => new { s.Date, s.Close, s.Open, s.High, s.Low }).OrderBy(d => d.Date)
                .ToListAsync();

            if (!quotes.Any())
            {
                return null;
            }

            var stockQuoteVm = await GetStockInternalAsync(ticker);
            stockQuoteVm.ChartQuotes = quotes.Select(s => new StockQuoteChartVm()
            {
                Date = s.Date.ToShortDateString(),
                Close = s.Close,
                Open = s.Open,
                Low = s.Low,
                High = s.High
            });
            return stockQuoteVm;
        }

        private async Task<StockWithChartVm> GetStockInternalAsync(string ticker)
        {
            ticker = ticker.ToUpperInvariant();
            var stockVm = await DbContext.Stocks.Select(s => new StockWithChartVm() { Ticker = s.Id, MinDate = s.MinDate, MaxDate = s.MaxDate }).FirstOrDefaultAsync(s => s.Ticker == ticker);
            if (stockVm == null)
            {
                return null;
            }

            var openClose = await DbContext.StockQuotes.Where(s => s.Ticker == ticker).Select(s => new { s.Close, s.Open, s.Date }).OrderByDescending(s => s.Date).FirstOrDefaultAsync();
            stockVm.Open = openClose.Open;
            stockVm.Close = openClose.Close;

            stockVm.AllTimeHigh = await DbContext.StockQuotes.Where(s => s.Ticker == ticker).MaxAsync(s => s.High);
            stockVm.AllTimeLow = await DbContext.StockQuotes.Where(s => s.Ticker == ticker).MinAsync(s => s.Low);

            var fiftyTwoWeekAgo = DateTime.Today.AddYears(-1);
            stockVm.FiftyTwoWeekHigh = await DbContext.StockQuotes.Where(s => s.Ticker == ticker && s.Date >= fiftyTwoWeekAgo).MaxAsync(s => s.High);
            stockVm.FiftyTwoWeekLow = await DbContext.StockQuotes.Where(s => s.Ticker == ticker && s.Date >= fiftyTwoWeekAgo).MinAsync(s => s.Low);

            return stockVm;
        }

        public async Task<StockVm> GetStockAsync(string ticker)
        {
            return await GetStockInternalAsync(ticker);
        }

        public async Task<AnalyzerResult> LoadStockAsync(string ticker)
        {
            var result = await _stockLoader.LoadFullAsync(ticker);
            return result;
        }
    }

    public class StockWithChartVm : StockVm
    {
        public IEnumerable<StockQuoteChartVm> ChartQuotes { get; set; }
    }

    public class StockQuoteChartVm
    {
        public string Date { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
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
        public decimal Open { get; set; }
        public decimal Close { get; set; }
    }
}