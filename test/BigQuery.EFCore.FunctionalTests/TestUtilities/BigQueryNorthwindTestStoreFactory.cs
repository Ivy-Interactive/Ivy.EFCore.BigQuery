using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities
{
    public class BigQueryNorthwindTestStoreFactory : BigQueryTestStoreFactory
    {
        public const string Name = "efc_northwind";
        public static readonly string NorthwindConnectionString = BigQueryTestStore.CreateConnectionString(Name);

        public new static BigQueryNorthwindTestStoreFactory Instance { get; } = new();

        protected BigQueryNorthwindTestStoreFactory()
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => BigQueryTestStore.GetOrCreate(Name, scriptPath: "Northwind.sql");
    }
}