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
        public async Task<IEnumerable<StockQuote>> LoadAsync(string ticker, string outputSize)
        {
            return Quotes;
        }
    }
}
