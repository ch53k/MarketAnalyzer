using System.Web.Http;
using MarketAnalyzer.Model;

namespace MarketAnalyzer.Shared
{
    public class AnalyzerControllerBase : ApiController
    {
        protected readonly AnalyzerDbContext Dbcontext;

        public AnalyzerControllerBase()
        {
            Dbcontext = new AnalyzerDbContext("DefaultConnection");
            Dbcontext.Database.Initialize(true);
        }

        protected override void Dispose(bool disposing)
        {
            Dbcontext.Dispose();
        }
    }
}