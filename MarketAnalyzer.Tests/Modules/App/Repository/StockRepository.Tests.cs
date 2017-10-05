using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketAnalyzer.Model;
using MarketAnalyzer.Shared;
using MarketAnalyzer.Tests.Model;
using MarketAnalyzer.Tests.Modules.App.Repository.TestFixtures;
using MarketAnalyzer.Tests.Shared;
using MarketAnalyzer.Tests.Shared.Stocks.TestFixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarketAnalyzer.Tests.Modules.App.Repository
{
    [TestClass]
    public class StockRepositoryTest
    {
        [TestMethod]
        public async Task GetTickers_Success_NoStocks()
        {
            var repository = StockRepositoryFactory.Create();
            var stocks = new List<Stock>();

            var mockDataset = MockDataSet.Get(stocks);
            repository.MockDbContext.Setup(c => c.Stocks).Returns(mockDataset.Object);

            var result = await repository.GetTickersAsync();
            Assert.AreEqual(result.Count(), stocks.Count);
        }

        [TestMethod]
        public async Task GetTickers_Success()
        {
            var repository = StockRepositoryFactory.Create();
            var stocks = new List<Stock>
            {
                new Stock()
                {
                    Id = "TICKER1",
                    LastLoadDate = DateTime.Today,
                    MaxDate = DateTime.Today,
                    MinDate = DateTime.Today.AddYears(-10)
                },
                new Stock()
                {
                    Id = "TICKER2",
                    LastLoadDate = DateTime.Today.AddDays(-1),
                    MaxDate = DateTime.Today.AddDays(-1),
                    MinDate = DateTime.Today.AddYears(-10).AddDays(-1)
                }
            };

            var mockDataset = MockDataSet.Get(stocks);
            repository.MockDbContext.Setup(c => c.Stocks).Returns(mockDataset.Object);

            var result = (await repository.GetTickersAsync()).ToList();
            Assert.AreEqual(result.Count, stocks.Count);
            Assert.IsTrue(result.Any(s => s == stocks.ElementAt(0).Id));
            Assert.IsTrue(result.Any(s => s == stocks.ElementAt(1).Id));
        }

        [TestMethod]
        public async Task StockRepository_GetStockVm_Success()
        {
            var dataFixture = new StockDataFixture();
            var repository = StockRepositoryFactory.Create();
            repository.MockDbContext.Setup(s => s.StockQuotes).Returns(dataFixture.QuoteDataFixture.MockDataSet.Object);
            repository.MockDbContext.Setup(s => s.Stocks).Returns(dataFixture.MockDataSet.Object);

            var ticker = dataFixture.Entities.First().Id;
            var result = await repository.GetStockAsync(ticker);

            var quotes = dataFixture.QuoteDataFixture.Entities.Where(s => s.Ticker == ticker).ToList();

            Assert.AreEqual(result.Ticker, ticker);
            var lastRecord = quotes.Where(s => s.Ticker == ticker).OrderByDescending(s => s.Date).First();
            Assert.AreEqual(result.Open, lastRecord.Open);
            Assert.AreEqual(result.Close, lastRecord.Close);

            Assert.AreEqual(result.AllTimeHigh, quotes.Where(s => s.Ticker == ticker).Max(s => s.High));
            Assert.AreEqual(result.AllTimeLow, quotes.Where(s => s.Ticker == ticker).Min(s => s.Low));

            var fiftyTwoWeekAgo = DateTime.Today.AddYears(-1);
            Assert.AreEqual(result.AllTimeHigh, quotes.Where(s => s.Ticker == ticker && s.Date >= fiftyTwoWeekAgo).Max(s => s.High));
            Assert.AreEqual(result.AllTimeLow, quotes.Where(s => s.Ticker == ticker && s.Date >= fiftyTwoWeekAgo).Min(s => s.Low));
        }

        [TestMethod]
        public async Task StockRepository_GetStockVm_Failed_BadTicker()
        {
            var dataFixture = new StockDataFixture();
            var repository = StockRepositoryFactory.Create();
            repository.MockDbContext.Setup(s => s.StockQuotes).Returns(dataFixture.QuoteDataFixture.MockDataSet.Object);
            repository.MockDbContext.Setup(s => s.Stocks).Returns(dataFixture.MockDataSet.Object);

            var ticker = dataFixture.Entities.First().Id;
            var result = await repository.GetStockAsync($"{ticker}123");
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task StockRepository_GetStockWithChartVm_ByDate_Success()
        {
            var dataFixture = new StockDataFixture();
            var repository = StockRepositoryFactory.Create();
            repository.MockDbContext.Setup(s => s.StockQuotes).Returns(dataFixture.QuoteDataFixture.MockDataSet.Object);
            repository.MockDbContext.Setup(s => s.Stocks).Returns(dataFixture.MockDataSet.Object);

            var ticker = dataFixture.Entities.First().Id;
            var startDate = new DateTime(2012, 1,1);
            var endDate = DateTime.Today;

            var result = await repository.GetChartAsync($"{ticker}", startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"));
            Assert.AreEqual(dataFixture.QuoteDataFixture.Entities.Count(s=>s.Ticker==ticker && s.Date>=startDate && s.Date<=endDate), result.ChartQuotes.Count());
            foreach (var chartQuote in result.ChartQuotes)
            {
                var orgQuote = dataFixture.QuoteDataFixture.Entities.First(s => s.Ticker == ticker && s.Date == DateTime.Parse(chartQuote.Date));
                Assert.AreEqual(chartQuote.Close, orgQuote.Close);
                Assert.AreEqual(chartQuote.High, orgQuote.High);
                Assert.AreEqual(chartQuote.Low, orgQuote.Low);
                Assert.AreEqual(chartQuote.Open, orgQuote.Open);
            }
        }

        [TestMethod]
        public async Task StockRepository_GetStockWithChartVm_ByDate_Failed_BadTicker()
        {
            var dataFixture = new StockDataFixture();
            var repository = StockRepositoryFactory.Create();
            repository.MockDbContext.Setup(s => s.StockQuotes).Returns(dataFixture.QuoteDataFixture.MockDataSet.Object);
            repository.MockDbContext.Setup(s => s.Stocks).Returns(dataFixture.MockDataSet.Object);

            var ticker = dataFixture.Entities.First().Id;
            var result = await repository.GetChartAsync($"{ticker}123", "20120101", "20161231");
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task StockRepository_GetStockWithChartVm_ByDate_Failed_FailedLoader()
        {
            var dataFixture = new StockDataFixture();
            var processor = new StockProcessorTestFixture();
            var repository = StockRepositoryFactory.Create(processor);
            repository.MockDbContext.Setup(s => s.StockQuotes).Returns(dataFixture.QuoteDataFixture.MockDataSet.Object);
            repository.MockDbContext.Setup(s => s.Stocks).Returns(dataFixture.MockDataSet.Object);

            processor.LoadCompactAsyncResult = AnalyzerResult.Fail("Failed to load.");
            var result = await repository.GetChartAsync("123", "20120101", "20161231");
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task StockRepository_GetStockWithChartVm_ByTake_Success()
        {
            var dataFixture = new StockDataFixture();
            var repository = StockRepositoryFactory.Create();
            repository.MockDbContext.Setup(s => s.StockQuotes).Returns(dataFixture.QuoteDataFixture.MockDataSet.Object);
            repository.MockDbContext.Setup(s => s.Stocks).Returns(dataFixture.MockDataSet.Object);

            var ticker = dataFixture.Entities.First().Id;
            var take = 3;
            var result = await repository.GetChartAsync($"{ticker}", take);
            Assert.AreEqual(take, result.ChartQuotes.Count());
            foreach (var chartQuote in result.ChartQuotes)
            {
                var orgQuote = dataFixture.QuoteDataFixture.Entities.Where(s=> s.Ticker == ticker).OrderByDescending(s=>s.Date).Take(take).First(s => s.Date == DateTime.Parse(chartQuote.Date));
                Assert.AreEqual(chartQuote.Close, orgQuote.Close);
                Assert.AreEqual(chartQuote.High, orgQuote.High);
                Assert.AreEqual(chartQuote.Low, orgQuote.Low);
                Assert.AreEqual(chartQuote.Open, orgQuote.Open);
            }
        }

        [TestMethod]
        public async Task StockRepository_GetStockWithChartVm_ByTake_Failed_BadTicker()
        {
            var dataFixture = new StockDataFixture();
            var repository = StockRepositoryFactory.Create();
            repository.MockDbContext.Setup(s => s.StockQuotes).Returns(dataFixture.QuoteDataFixture.MockDataSet.Object);
            repository.MockDbContext.Setup(s => s.Stocks).Returns(dataFixture.MockDataSet.Object);

            var ticker = dataFixture.Entities.First().Id;
            var result = await repository.GetChartAsync($"{ticker}123", 30);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task StockRepository_GetStockWithChartVm_ByTake_Failed_FailedLoader()
        {
            var dataFixture = new StockDataFixture();
            var processor = new StockProcessorTestFixture();
            var repository = StockRepositoryFactory.Create(processor);
            repository.MockDbContext.Setup(s => s.StockQuotes).Returns(dataFixture.QuoteDataFixture.MockDataSet.Object);
            repository.MockDbContext.Setup(s => s.Stocks).Returns(dataFixture.MockDataSet.Object);
            
            processor.LoadCompactAsyncResult = AnalyzerResult.Fail("Failed to load.");
            var result = await repository.GetChartAsync("ticker", 30);
            Assert.IsNull(result);
        }
    }
}
