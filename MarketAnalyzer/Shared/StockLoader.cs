using System;
using System.Data.Entity;
using System.Net.Http;
using System.Threading.Tasks;
using MarketAnalyzer.Model;
using Newtonsoft.Json.Linq;

namespace MarketAnalyzer.Shared
{
    public class StockLoader
    { 
        public static async Task<AnalyzerResult> Load(AnalyzerDbContext dbContext, string ticker)
        {
            ticker = ticker.ToUpperInvariant();
            var stock = await dbContext.Stocks.FirstOrDefaultAsync(s => s.Id == ticker);
            var intialStockLoad = false;
            if (stock == null)
            {
                stock = dbContext.Stocks.Add(new Stock() {Id = ticker, MinDate = DateTime.MaxValue, MaxDate = DateTime.MinValue });
                intialStockLoad = true;
            }

            string quotesSerialized;
            var retryCount = 0;
            while (true)
            {
                using (var httpClient = new HttpClient())
                {
                    var result = await httpClient.GetAsync(
                        $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={ticker}&outputsize=full&apikey={Globals.StockQuoteKey}");
                    if (result.IsSuccessStatusCode)
                    {
                        quotesSerialized = await result.Content.ReadAsStringAsync();
                        break;
                    }
                    retryCount++;
                }
                if (retryCount == 2)
                {
                    return AnalyzerResult.Fail("Unable to get quotes from remote service.");
                }
            }
            var o = JObject.Parse(quotesSerialized);
                var metaData = o.First;
                var timeSeries = metaData.Next;
                var day = timeSeries.First;

                foreach (var child in day.Children())
                {
                    var property = (JProperty) child;
                    var stockQuote = new StockQuote()
                    {
                        Ticker = ticker,
                        Date = DateTime.Parse(property.Name)
                    };
                    
                    if (!intialStockLoad && (stock.MinDate <= stockQuote.Date || stock.MaxDate >= stockQuote.Date))
                    {
                        continue;
                    }

                    stock.MinDate = stock.MinDate > stockQuote.Date ? stockQuote.Date : stock.MinDate;
                    stock.MaxDate = stock.MaxDate < stockQuote.Date ? stockQuote.Date : stock.MaxDate;

                    foreach (var dayChild in property.First.Children<JProperty>())
                    {
                        switch (dayChild.Name)
                        {
                            case "1. open":
                            {
                                stockQuote.Open = decimal.Parse(dayChild.First.ToString());
                                break;
                            }
                            case "2. high":
                            {
                                stockQuote.High = decimal.Parse(dayChild.First.ToString());
                                break;
                            }
                            case "3. low":
                            {
                                stockQuote.Low = decimal.Parse(dayChild.First.ToString());
                                break;
                            }
                            case "4. close":
                            {
                                stockQuote.Close = decimal.Parse(dayChild.First.ToString());
                                break;
                            }
                            case "5. adjusted close":
                            {
                                stockQuote.AdjClose = decimal.Parse(dayChild.First.ToString());
                                break;
                            }
                            case "6. volume":
                            {
                                stockQuote.Volume = int.Parse(dayChild.First.ToString());
                                break;
                            }
                            case "7. dividend amount":
                            {
                                stockQuote.DividendAmount = decimal.Parse(dayChild.First.ToString());
                                break;
                            }
                            case "8. split coefficient":
                            {
                                stockQuote.SplitCoefficient = decimal.Parse(dayChild.First.ToString());
                                break;
                            }
                        }
                    }
                    dbContext.StockQuotes.Add(stockQuote);
                }
                stock.LastLoadDate = DateTime.UtcNow;
               await dbContext.SaveChangesAsync();

                return AnalyzerResult.Sucess();
        }
    }
}