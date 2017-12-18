using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MarketAnalyzer.Model;
using Newtonsoft.Json.Linq;

namespace MarketAnalyzer.Shared.Stocks
{
    public class StockLoaderAlphaVantage : IStockLoader
    {
        private readonly string _stockConnectionString;

        public StockLoaderAlphaVantage(ConnectionStrings connectionStrings)
        {
            _stockConnectionString = connectionStrings.StockQuoteKey;
        }

        protected virtual async Task<string> GetAsync(string ticker, string outputSize)
        {
            string quotesSerialized;
            var retryCount = 0;
            while (true)
            {
                using (var httpClient = new HttpClient())
                {
                    var result = await httpClient.GetAsync(
                        $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={ticker}&outputsize={outputSize}&apikey={_stockConnectionString}");
                    if (result.IsSuccessStatusCode)
                    {
                        quotesSerialized = await result.Content.ReadAsStringAsync();
                        break;
                    }
                    retryCount++;
                }
                if (retryCount == 2)
                {
                    throw new Exception("Unable to get quotes from remote service.");
                }
            }
            return quotesSerialized;
        }

        public async Task<IEnumerable<StockQuote>> LoadAsync(string ticker, string outputSize)
        {
            ticker = ticker.ToUpperInvariant();

            var stockQuotes = new List<StockQuote>();

            string quotesSerialized = await GetAsync(ticker, outputSize);
            
            var o = JObject.Parse(quotesSerialized);
            var metaData = o.First;
            var timeSeries = metaData.Next;
            if (timeSeries == null)
            {
                throw new Exception($"Ticker {ticker} returned 0 quotes.");
            }
            var day = timeSeries.First;

            foreach (var child in day.Children())
            {
                var property = (JProperty) child;
                var stockQuote = new StockQuote()
                {
                    Ticker = ticker,
                    Date = DateTime.Parse(property.Name)
                };

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
                stockQuotes.Add(stockQuote);
            }
            return stockQuotes;
        }
    }
}