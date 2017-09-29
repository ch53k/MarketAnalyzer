namespace MarketAnalyzer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockQuotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ticker = c.String(nullable: false, maxLength: 128),
                        Open = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Close = c.Decimal(nullable: false, precision: 18, scale: 2),
                        High = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Low = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdjClose = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Volume = c.Int(nullable: false),
                        SplitCoefficient = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        DividendAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stocks", t => t.Ticker)
                .Index(t => t.Ticker);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockQuotes", "Ticker", "dbo.Stocks");
            DropIndex("dbo.StockQuotes", new[] { "Ticker" });
            DropTable("dbo.Stocks");
            DropTable("dbo.StockQuotes");
        }
    }
}
