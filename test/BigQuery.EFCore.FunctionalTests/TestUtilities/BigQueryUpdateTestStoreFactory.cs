using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities
{
    public class BigQueryUpdateTestStoreFactory : BigQueryTestStoreFactory
    {
        public const string Name = "BigQueryUpdateTest";

        public new static BigQueryUpdateTestStoreFactory Instance { get; } = new();

        protected BigQueryUpdateTestStoreFactory()
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => BigQueryTestStore.GetOrCreate(Name);
    }
}
