using Microsoft.EntityFrameworkCore.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities
{
    internal class BigQueryNorthwindTestStoreFactory : BigQueryTestStoreFactory
    {
        public const string Name = "Northwind";
        public static readonly string NorthwindConnectionString = BigQueryTestStore.CreateConnectionString(Name);
        public static new BigQueryNorthwindTestStoreFactory Instance { get; } = new();

        protected BigQueryNorthwindTestStoreFactory()
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => BigQueryTestStore.GetOrCreateWithScriptPath(storeName, seedScriptPath : "Northwind.sql", refreshScriptPath : "Northwind.clone.sql");
    }
}
