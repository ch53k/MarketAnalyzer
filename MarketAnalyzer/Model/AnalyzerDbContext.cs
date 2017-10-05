using System;
using System.Data.Entity;

namespace MarketAnalyzer.Model
{
    public class AnalyzerDbContext : DbContext
    {
        public AnalyzerDbContext(string connectionName) : base(connectionName)
        {
            
        }

#if DEBUG
        public AnalyzerDbContext() : base("DefaultConnection")
        {
            
        }
#endif
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<StockQuote> StockQuotes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>().HasMany(s=>s.Quotes).WithRequired(s=>s.Stock).HasForeignKey(s=>s.Ticker).WillCascadeOnDelete(false);
        }
    }
}
