using MarketAnalyzer.Model;
using MarketAnalyzer.Modules.App.Repository;
using MarketAnalyzer.Shared.Stocks;
using MarketAnalyzer.Tests.Shared;
using MarketAnalyzer.Tests.Shared.Stocks.TestFixtures;
using Moq;

namespace MarketAnalyzer.Tests.Modules.App.Repository.TestFixtures
{
    public class StockRepositoryTestFixture : StockRepository
    {
        public Mock<AnalyzerDbContext> MockDbContext { get; }

        public StockRepositoryTestFixture(Mock<AnalyzerDbContext> mockDbContext) : this(mockDbContext, new StockProcessorTestFixture())
        {
        }

        public StockRepositoryTestFixture(Mock<AnalyzerDbContext> mockDbContext, StockProcessorTestFixture stockProcessor) : base(mockDbContext.Object, stockProcessor)
        {
            MockDbContext = mockDbContext;
        }
    }
}
