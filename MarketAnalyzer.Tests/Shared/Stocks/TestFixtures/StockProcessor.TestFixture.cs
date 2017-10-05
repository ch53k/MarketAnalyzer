using System.Threading.Tasks;
using MarketAnalyzer.Model;
using MarketAnalyzer.Shared;
using MarketAnalyzer.Shared.Stocks;
using Moq;

namespace MarketAnalyzer.Tests.Shared.Stocks.TestFixtures
{
    public class StockProcessorTestFixture : StockProcessor
    {
        public AnalyzerResult LoadCompactAsyncResult { get; set; }
        public AnalyzerResult LoadFullAsyncResult { get; set; }

        public bool IsMarketClosedValue = true;
        protected override bool IsMarketClosed => IsMarketClosedValue;

        public StockProcessorTestFixture() : base(new Mock<AnalyzerDbContext>().Object, new StockLoaderTestFixture())
        {
        }
        public StockProcessorTestFixture(Mock<AnalyzerDbContext> mockDbContext, StockLoaderTestFixture stockLoader) : base(mockDbContext.Object, stockLoader)
        {
        }

        public override async Task<AnalyzerResult> LoadCompactAsync(string ticker)
        {
            if (LoadCompactAsyncResult == null)
            {
                return await base.LoadCompactAsync(ticker);
            }
            return LoadCompactAsyncResult;
        }

        public override async Task<AnalyzerResult> LoadFullAsync(string ticker)
        {
            if (LoadFullAsyncResult == null)
            {
                return await base.LoadFullAsync(ticker);
            }
            return LoadFullAsyncResult;
        }
    }
}
