using System;
using System.Collections.Generic;
using MarketAnalyzer.Model;

namespace MarketAnalyzer.Tests.Model
{
    public class StockDataFixture : DataFixtureBase<Stock>
    {
        public StockQuoteDataFixture QuoteDataFixture { get; }

        public StockDataFixture()
        {
            QuoteDataFixture = new StockQuoteDataFixture();
            Create();
        }

        public List<Stock> Create()
        {
            var stocks = new List<Stock>
            {
                new Stock()
                {
                    Id = "TICKER1",
                    LastLoadDate = DateTime.Today,
                    MaxDate = DateTime.Today,
                    MinDate = DateTime.Today.AddYears(-10),
                    Quotes = QuoteDataFixture.Create(10, "TICKER1")
                },
                new Stock()
                {
                    Id = "TICKER2",
                    LastLoadDate = DateTime.Today.AddDays(-1),
                    MaxDate = DateTime.Today.AddDays(-1),
                    MinDate = DateTime.Today.AddYears(-10).AddDays(-1),
                    Quotes = QuoteDataFixture.Create(10, "TICKER2")
                }
            };
            Entities = stocks;
            return stocks;
        }
    }
}
