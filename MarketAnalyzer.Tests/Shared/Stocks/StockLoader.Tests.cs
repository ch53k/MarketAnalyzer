using System.Linq;
using System.Threading.Tasks;
using MarketAnalyzer.Tests.Shared.Stocks.TestFixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarketAnalyzer.Tests.Shared.Stocks
{
    [TestClass]
    public class StockLoaderTests
    {
        [TestMethod]
        public async Task Full_Success()
        {
           var loader = new StockLoaderAlphaVantageTestFixture();
            var quotes = (await loader.LoadAsync("MSFT", "full")).ToList();
            Assert.AreEqual(quotes.Count, 100);
            foreach (var quote in quotes)
            {
                Assert.IsTrue(quote.Close > 0);
                Assert.IsTrue(quote.High > 0);
                Assert.IsTrue(quote.Low > 0);
                Assert.IsTrue(quote.AdjClose > 0);
                Assert.IsTrue(quote.DividendAmount >= 0);
                Assert.IsTrue(quote.SplitCoefficient >= 0);
                Assert.IsTrue(quote.Volume >= 0);
            }
        }
        
    }
}
