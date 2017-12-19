using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MarketAnalyzer.Model;

namespace MarketAnalyzer.Shared.Stocks
{
    public class StockProcessor : IStockProcessor
    {
        private readonly AnalyzerDbContext _dbContext;
        private readonly IStockLoader _loader;

        protected virtual bool IsMarketClosed
        {
            get
            {
                var minTimeToGetClose = new TimeSpan(16, 6, 0);
                var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")).TimeOfDay;
                return currentTime > minTimeToGetClose;
            }
        }


        public StockProcessor(AnalyzerDbContext dbContext, IStockLoader loader)
        {
            _dbContext = dbContext;
            _loader = loader;
        }
        public virtual async Task<AnalyzerResult> LoadCompactAsync(string ticker)
        {
            return await LoadAsync(ticker, "compact");
        }

        public virtual async Task<AnalyzerResult> LoadFullAsync(string ticker)
        {
            return await LoadAsync(ticker, "full");
        }

        protected async Task<AnalyzerResult> LoadAsync(string ticker, string outputSize)
        {
            ticker = ticker.ToUpperInvariant();


            var stock = await _dbContext.Stocks.FirstOrDefaultAsync(s => s.Id == ticker);
            var initialStockLoad = false;
            if (stock == null)
            {
                stock = _dbContext.Stocks.Add(new Stock()
                {
                    Id = ticker,
                    MinDate = DateTime.MaxValue,
                    MaxDate = DateTime.MinValue
                });
                initialStockLoad = true;
            }
            if (stock.MaxDate == DateTime.Today.AddDays(-1) && !IsMarketClosed)
            {
                return AnalyzerResult.Sucess();
            }

            var test = 0;
            try
            {
                var quotes = await _loader.LoadAsync(ticker, outputSize);
                foreach (var quote in quotes.OrderBy(s=>s.Date).Where(q=> initialStockLoad || q.Date<stock.MinDate || q.Date.Date>stock.MaxDate.Date))
                {
                    if (quote.Date == DateTime.Today && !IsMarketClosed)
                    {
                        continue;
                    }

                    _dbContext.StockQuotes.Add(quote);
                    test++;

                    stock.MinDate = stock.MinDate > quote.Date ? quote.Date : stock.MinDate;
                    stock.MaxDate = stock.MaxDate < quote.Date ? quote.Date : stock.MaxDate;
                }
            }
            catch (Exception e)
            {
                return AnalyzerResult.Fail(e.Message);
            }

            stock.LastLoadDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            
            return AnalyzerResult.Sucess();
        }
    }
}