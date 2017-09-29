namespace MarketAnalyzer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.StockQuotes", new[] { "Ticker" });
            AddColumn("dbo.Stocks", "LastLoadDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Stocks", "MinDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Stocks", "MaxDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.StockQuotes", "Id");
            CreateIndex("dbo.StockQuotes", "Ticker");
            CreateIndex("dbo.StockQuotes", "Date");
        }
        
        public override void Down()
        {
            DropIndex("dbo.StockQuotes", new[] { "Date" });
            DropIndex("dbo.StockQuotes", new[] { "Ticker" });
            DropIndex("dbo.StockQuotes", new[] { "Id" });
            DropColumn("dbo.Stocks", "MaxDate");
            DropColumn("dbo.Stocks", "MinDate");
            DropColumn("dbo.Stocks", "LastLoadDate");
            CreateIndex("dbo.StockQuotes", "Ticker");
        }
    }
}
