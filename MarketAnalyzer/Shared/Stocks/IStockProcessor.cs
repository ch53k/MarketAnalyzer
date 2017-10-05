using System.Threading.Tasks;

namespace MarketAnalyzer.Shared.Stocks
{
    public interface IStockProcessor
    {
        Task<AnalyzerResult> LoadCompactAsync(string ticker);
        Task<AnalyzerResult> LoadFullAsync(string ticker);
    }
}