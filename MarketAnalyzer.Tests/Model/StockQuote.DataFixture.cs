using System;
using System.Collections.Generic;
using System.Linq;
using MarketAnalyzer.Model;

namespace MarketAnalyzer.Tests.Model
{
    public class StockQuoteDataFixture :DataFixtureBase<StockQuote>
    {
        public StockQuoteDataFixture()
        {
            Create(10);
        }
        public List<StockQuote> Create(int count, string ticker = null)
        {
            var rnd = new Random();
            var quotes = new List<StockQuote>();
            for (var i = 0; i < count; i++)
            {
                var quote = new StockQuote()
                {
                    Id = i + 1,
                    Ticker = ticker ?? GetRandomTicker(),
                    Date = DateTime.Today.AddDays(-i),
                    Open = (decimal) rnd.NextDouble(),
                    Close = (decimal) rnd.NextDouble(),
                    High = (decimal) rnd.NextDouble(),
                    Low = (decimal) rnd.NextDouble(),
                    AdjClose = (decimal) rnd.NextDouble(),
                    DividendAmount = (decimal) rnd.NextDouble(),
                    SplitCoefficient = (decimal) rnd.NextDouble(),
                    Volume = rnd.Next()
                };
                quotes.Add(quote);
            }
            if (Entities == null)
            {
                Entities = new List<StockQuote>();
            }

            while (Entities.Any(s => s.Ticker == ticker))
            {
                var quote = Entities.FirstOrDefault(s => s.Ticker == ticker);
                Entities.Remove(quote);
            }
            Entities.AddRange(quotes);
            return quotes;
        }

        private string GetRandomTicker()
        {
            var tickers = new List<string>()
            {
                "MSFT", "GOOG", "INTEL", "GE", "GM", "TH", "GOOD", "ROIC", "BKH", "RG"
            };
            var rnd = new Random();
            return tickers.ElementAt(rnd.Next(0, tickers.Count - 1));
        }
    }
}
