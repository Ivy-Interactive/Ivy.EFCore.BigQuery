using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities
{
    public class BigQueryNorthwindTestStoreFactory : BigQueryTestStoreFactory
    {
        public new static BigQueryNorthwindTestStoreFactory Instance { get; } = new();

        protected BigQueryNorthwindTestStoreFactory()
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => BigQueryTestStore.GetOrCreate(storeName, scriptPath: "Northwind.sql");
    }
}