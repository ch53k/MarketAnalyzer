using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace MarketAnalyzer.Tests.Model
{
    public abstract class DataFixtureBase<T> where T : class
    {
        public List<T> Entities { get; protected set; }

        public Mock<DbSet<T>> MockDataSet => Model.MockDataSet.Get(Entities);
    }
}
