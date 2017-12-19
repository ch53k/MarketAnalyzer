using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketAnalyzer.Model;
using MarketAnalyzer.Shared.Stocks;
using MarketAnalyzer.Tests.Model;
using MarketAnalyzer.Tests.Shared.Stocks.TestFixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MarketAnalyzer.Tests.Shared.Stocks
{
    [TestClass]
    public class StockProcessorTests
    {
        [TestMethod]
        public async Task Full_Success()
        {
            var mockDbContext = new Mock<AnalyzerDbContext>();
            var mockStockQuoteDataSet = MockDataSet.Get(new List<StockQuote>());
            mockDbContext.Setup(s => s.StockQuotes).Returns(mockStockQuoteDataSet.Object);

            var mockStockDataSet = MockDataSet.Get(new List<Stock>());
            mockStockDataSet.Setup(s => s.Add(It.IsAny<Stock>())).Returns((Stock s) => s);
            mockDbContext.Setup(s => s.Stocks).Returns(mockStockDataSet.Object);

            var stockLoader = new StockLoaderTestFixture();
            var processor = new StockProcessorTestFixture(mockDbContext, stockLoader) { IsMarketClosedValue = true };

            var stockQuoteDataFixture = new StockQuoteDataFixture();
            var ticker = "goog";
            var numberofQuotes = 10;
            stockQuoteDataFixture.Create(numberofQuotes, ticker);

            stockLoader.Quotes = stockQuoteDataFixture.Entities.Where(s=>s.Ticker==ticker).ToList();
            var result = await processor.LoadFullAsync(ticker);

            mockStockQuoteDataSet.Verify(s=>s.Add(It.IsAny<StockQuote>()), Times.Exactly(10));
            mockStockDataSet.Verify(s => s.Add(It.IsAny<Stock>()), Times.Once);
            mockDbContext.Verify(s=>s.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(result.Succeeded);
        }
        [TestMethod]
        public async Task Full_Success_NotInitialLoad()
        {
            var mockDbContext = new Mock<AnalyzerDbContext>();

            var stockDataFixture = new StockDataFixture();
            var stock = stockDataFixture.Entities.First();
            var ticker = stock.Id;
            stock.LastLoadDate = DateTime.Today.AddDays(-1);
            stock.MaxDate = stock.LastLoadDate;

            var mockStockDataSet = stockDataFixture.MockDataSet;
            mockStockDataSet.Setup(s => s.Add(It.IsAny<Stock>())).Returns((Stock s) => s);
            mockDbContext.Setup(s => s.Stocks).Returns(mockStockDataSet.Object);
            
            stockDataFixture.QuoteDataFixture.Entities.Remove(stockDataFixture.QuoteDataFixture.Entities.First(s=>s.Ticker==ticker && s.Date==DateTime.Today));
            var mockStockQuoteDataSet = stockDataFixture.QuoteDataFixture.MockDataSet;
            mockDbContext.Setup(s => s.StockQuotes).Returns(mockStockQuoteDataSet.Object);


            var stockLoader = new StockLoaderTestFixture();
            var processor = new StockProcessorTestFixture(mockDbContext, stockLoader) { IsMarketClosedValue = true };


            stockLoader.Quotes = stockDataFixture.QuoteDataFixture.Entities.Where(s=>s.Ticker==ticker).ToList();
            stockLoader.Quotes.Add(new StockQuote()
            {
                Ticker = ticker,
                Date = DateTime.Today
            });
            var result = await processor.LoadFullAsync(ticker);

            mockStockQuoteDataSet.Verify(s=>s.Add(It.IsAny<StockQuote>()), Times.Once);
            mockStockDataSet.Verify(s=>s.Add(It.IsAny<Stock>()), Times.Never);
            mockDbContext.Verify(s=>s.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(stock.MaxDate, DateTime.Today);
        }
        [TestMethod]
        public async Task Compact_Success_InitialLoad()
        {
            var mockDbContext = new Mock<AnalyzerDbContext>();
            var mockStockQuoteDataSet = MockDataSet.Get(new List<StockQuote>());
            mockDbContext.Setup(s => s.StockQuotes).Returns(mockStockQuoteDataSet.Object);

            var mockStockDataSet = MockDataSet.Get(new List<Stock>());
            mockStockDataSet.Setup(s => s.Add(It.IsAny<Stock>())).Returns((Stock s) => s);
            mockDbContext.Setup(s => s.Stocks).Returns(mockStockDataSet.Object);

            var stockLoader = new StockLoaderTestFixture();
            var processor = new StockProcessorTestFixture(mockDbContext, stockLoader) { IsMarketClosedValue = true };

            var stockQuoteDataFixture = new StockQuoteDataFixture();
            var ticker = "goog";
            var numberofQuotes = 10;
            stockQuoteDataFixture.Create(numberofQuotes, ticker);

            stockLoader.Quotes = stockQuoteDataFixture.Entities.Where(s=>s.Ticker==ticker).ToList();
            var result = await processor.LoadCompactAsync(ticker);

            mockStockQuoteDataSet.Verify(s => s.Add(It.IsAny<StockQuote>()), Times.Exactly(stockLoader.Quotes.Count()));
            mockStockDataSet.Verify(s => s.Add(It.IsAny<Stock>()), Times.Once);
            mockDbContext.Verify(s=>s.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(result.Succeeded);
        }
        [TestMethod]
        public async Task Compact_Success_NotInitialLoad()
        {
            var mockDbContext = new Mock<AnalyzerDbContext>();

            var stockDataFixture = new StockDataFixture();
            var stock = stockDataFixture.Entities.First();
            var ticker = stock.Id;
            stock.LastLoadDate = DateTime.Today.AddDays(-1);
            stock.MaxDate = stock.LastLoadDate;

            var mockStockDataSet = stockDataFixture.MockDataSet;
            mockStockDataSet.Setup(s => s.Add(It.IsAny<Stock>())).Returns((Stock s) => s);
            mockDbContext.Setup(s => s.Stocks).Returns(mockStockDataSet.Object);
            
            stockDataFixture.QuoteDataFixture.Entities.Remove(stockDataFixture.QuoteDataFixture.Entities.First(s=>s.Ticker==ticker && s.Date==DateTime.Today));
            var mockStockQuoteDataSet = stockDataFixture.QuoteDataFixture.MockDataSet;
            mockDbContext.Setup(s => s.StockQuotes).Returns(mockStockQuoteDataSet.Object);


            var stockLoader = new StockLoaderTestFixture();
            var processor = new StockProcessorTestFixture(mockDbContext, stockLoader) { IsMarketClosedValue = true };


            stockLoader.Quotes = stockDataFixture.QuoteDataFixture.Entities.Where(s=>s.Ticker==ticker).ToList();
            stockLoader.Quotes.Add(new StockQuote()
            {
                Ticker = ticker,
                Date = DateTime.Today
            });
            var result = await processor.LoadCompactAsync(ticker);

            mockStockQuoteDataSet.Verify(s=>s.Add(It.IsAny<StockQuote>()), Times.Once);
            mockStockDataSet.Verify(s=>s.Add(It.IsAny<Stock>()), Times.Never);
            mockDbContext.Verify(s=>s.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(stock.MaxDate, DateTime.Today);
        }
        [TestMethod]
        public async Task Process_Success_MarketNotClosed()
        {
            var mockDbContext = new Mock<AnalyzerDbContext>();

            var stockDataFixture = new StockDataFixture();
            var stock = stockDataFixture.Entities.First();
            var ticker = stock.Id;
            stock.LastLoadDate = DateTime.Today.AddDays(-1);
            stock.MaxDate = stock.LastLoadDate;

            var mockStockDataSet = stockDataFixture.MockDataSet;
            mockStockDataSet.Setup(s => s.Add(It.IsAny<Stock>())).Returns((Stock s) => s);
            mockDbContext.Setup(s => s.Stocks).Returns(mockStockDataSet.Object);
            
            stockDataFixture.QuoteDataFixture.Entities.Remove(stockDataFixture.QuoteDataFixture.Entities.First(s=>s.Ticker==ticker && s.Date==DateTime.Today));
            var mockStockQuoteDataSet = stockDataFixture.QuoteDataFixture.MockDataSet;
            mockDbContext.Setup(s => s.StockQuotes).Returns(mockStockQuoteDataSet.Object);


            var stockLoader = new StockLoaderTestFixture();
            var processor = new StockProcessorTestFixture(mockDbContext, stockLoader)
            {
                IsMarketClosedValue = false
            };


            stockLoader.Quotes = stockDataFixture.QuoteDataFixture.Entities.Where(s=>s.Ticker==ticker).ToList();
            stockLoader.Quotes.Add(new StockQuote()
            {
                Ticker = ticker,
                Date = DateTime.Today
            });
            var result = await processor.LoadCompactAsync(ticker);

            mockStockQuoteDataSet.Verify(s=>s.Add(It.IsAny<StockQuote>()), Times.Never);
            mockStockDataSet.Verify(s=>s.Add(It.IsAny<Stock>()), Times.Never);
            mockDbContext.Verify(s=>s.SaveChangesAsync(), Times.Never);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(stock.MaxDate, DateTime.Today.AddDays(-1));
        }
        [TestMethod]
        public async Task Process_Success_MarketNotClosed_MaxDayMinus2()
        {
            var mockDbContext = new Mock<AnalyzerDbContext>();

            var stockDataFixture = new StockDataFixture();
            var stock = stockDataFixture.Entities.First();
            var ticker = stock.Id;
            stock.LastLoadDate = DateTime.Today.AddDays(-2);
            stock.MaxDate = stock.LastLoadDate;

            var mockStockDataSet = stockDataFixture.MockDataSet;
            mockStockDataSet.Setup(s => s.Add(It.IsAny<Stock>())).Returns((Stock s) => s);
            mockDbContext.Setup(s => s.Stocks).Returns(mockStockDataSet.Object);
            
            stockDataFixture.QuoteDataFixture.Entities.Remove(stockDataFixture.QuoteDataFixture.Entities.First(s=>s.Ticker==ticker && s.Date==DateTime.Today));
            stockDataFixture.QuoteDataFixture.Entities.Remove(stockDataFixture.QuoteDataFixture.Entities.First(s=>s.Ticker==ticker && s.Date==DateTime.Today.AddDays(-1)));
            var mockStockQuoteDataSet = stockDataFixture.QuoteDataFixture.MockDataSet;
            mockDbContext.Setup(s => s.StockQuotes).Returns(mockStockQuoteDataSet.Object);


            var stockLoader = new StockLoaderTestFixture();
            var processor = new StockProcessorTestFixture(mockDbContext, stockLoader)
            {
                IsMarketClosedValue = false
            };


            stockLoader.Quotes = stockDataFixture.QuoteDataFixture.Entities.Where(s=>s.Ticker==ticker).ToList();
            stockLoader.Quotes.Add(new StockQuote()
            {
                Ticker = ticker,
                Date = DateTime.Today.AddDays(-1)
            });
            stockLoader.Quotes.Add(new StockQuote()
            {
                Ticker = ticker,
                Date = DateTime.Today
            });
            var result = await processor.LoadCompactAsync(ticker);

            mockStockQuoteDataSet.Verify(s=>s.Add(It.IsAny<StockQuote>()), Times.Once);
            mockStockDataSet.Verify(s=>s.Add(It.IsAny<Stock>()), Times.Never);
            mockDbContext.Verify(s=>s.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(stock.MaxDate, DateTime.Today.AddDays(-1));
        }
    }
}
