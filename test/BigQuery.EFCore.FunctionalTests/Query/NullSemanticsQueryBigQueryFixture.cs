using Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Ivy.EFCore.BigQuery.FunctionalTests.Query
{
    public class NullSemanticsQueryBigQueryFixture : NullSemanticsQueryFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory => BigQueryTestStoreFactory.Instance;
    }
}