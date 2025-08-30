using Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Ivy.EFCore.BigQuery.FunctionalTests.Query
{
    public class NorthwindQueryBigQueryFixture<TModelCustomizer> : NorthwindQueryRelationalFixture<TModelCustomizer>
        where TModelCustomizer : ITestModelCustomizer, new()
    {
        protected override ITestStoreFactory TestStoreFactory
            => BigQueryNorthwindTestStoreFactory.Instance;
    }

    public class NorthwindQueryBigQueryFixture : NorthwindQueryBigQueryFixture<BigQueryNorthwindModelCustomizer>
    {
    }
}