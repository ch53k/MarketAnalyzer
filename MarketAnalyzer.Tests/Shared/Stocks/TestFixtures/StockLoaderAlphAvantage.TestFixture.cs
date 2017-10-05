using System;
using System.IO;
using System.Threading.Tasks;
using MarketAnalyzer.Shared.Stocks;

namespace MarketAnalyzer.Tests.Shared.Stocks.TestFixtures
{
    public class StockLoaderAlphaVantageTestFixture : StockLoaderAlphaVantage
    {
        public StockLoaderAlphaVantageTestFixture() : base(new ConnectionStrings())
        {
        }

        protected override async Task<string> GetAsync(string ticker, string outputSize)
        {
            var executionDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent;
            var path = Path.Combine(executionDirectoryInfo?.FullName ?? "", "Shared\\Stocks\\Data\\stockData.alphavantage.json");
            var json = System.IO.File.ReadAllText(path);
            return json;
        }
    }
}
