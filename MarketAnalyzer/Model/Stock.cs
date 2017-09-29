using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketAnalyzer.Model
{
    public class Stock
    {
        [Key]
        public string Id { get; set; }
        public DateTime LastLoadDate { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public ICollection<StockQuote> Quotes { get; set; }
    }
}
