using Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Product = Microsoft.EntityFrameworkCore.TestModels.Northwind.Product;

namespace Ivy.EFCore.BigQuery.FunctionalTests.Query
{
    public class NorthwindQueryBigQueryFixture<TModelCustomizer> : NorthwindQueryRelationalFixture<TModelCustomizer>
        where TModelCustomizer : ITestModelCustomizer, new()
    {
        protected override ITestStoreFactory TestStoreFactory
            => BigQueryNorthwindTestStoreFactory.Instance;
    }
}
