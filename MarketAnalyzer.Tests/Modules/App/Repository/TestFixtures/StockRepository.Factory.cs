using MarketAnalyzer.Model;
using MarketAnalyzer.Shared;
using MarketAnalyzer.Tests.Shared;
using MarketAnalyzer.Tests.Shared.Stocks.TestFixtures;
using Moq;

namespace MarketAnalyzer.Tests.Modules.App.Repository.TestFixtures
{
    public static class StockRepositoryFactory
    {
        public static StockRepositoryTestFixture Create()
        {
            var mockDbContext = new Mock<AnalyzerDbContext>();
            var processor = new StockProcessorTestFixture(mockDbContext, new StockLoaderTestFixture())
            {
                LoadCompactAsyncResult = AnalyzerResult.Sucess(),
                LoadFullAsyncResult = AnalyzerResult.Sucess()
            };
            return new StockRepositoryTestFixture(mockDbContext, processor);
        }
        public static StockRepositoryTestFixture Create(StockProcessorTestFixture stockProcessor)
        {
            var mockDbContext = new Mock<AnalyzerDbContext>();
            return new StockRepositoryTestFixture(mockDbContext, stockProcessor);
        }
    }
}
