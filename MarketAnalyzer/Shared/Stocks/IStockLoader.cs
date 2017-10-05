using System.Collections.Generic;
using System.Threading.Tasks;
using MarketAnalyzer.Model;

namespace MarketAnalyzer.Shared.Stocks
{
    public interface IStockLoader
    {
        Task<IEnumerable<StockQuote>> LoadAsync(string ticker, string outputSize);
    }
}