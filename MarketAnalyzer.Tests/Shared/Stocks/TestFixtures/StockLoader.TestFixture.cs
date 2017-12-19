using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketAnalyzer.Model;
using MarketAnalyzer.Shared.Stocks;

namespace MarketAnalyzer.Tests.Shared.Stocks.TestFixtures
{
    public class StockLoaderTestFixture : IStockLoader
    {
        public List<StockQuote> Quotes { get; set; }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IEnumerable<StockQuote>> LoadAsync(string ticker, string outputSize)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return Quotes;
        }
    }
}
