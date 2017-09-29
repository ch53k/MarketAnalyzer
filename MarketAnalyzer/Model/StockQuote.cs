using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketAnalyzer.Model
{
    public class StockQuote
    {
        [Index]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index]
        public string Ticker { get; set; }

        [Index]
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal AdjClose { get; set; }
        public int Volume { get; set; }
        public decimal SplitCoefficient { get; set; }
        public decimal DividendAmount { get; set; }

        public Stock Stock { get; set; }
    }
}
