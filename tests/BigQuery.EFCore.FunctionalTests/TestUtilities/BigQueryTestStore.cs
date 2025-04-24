using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Ivy.EFCore.BigQuery.Extensions;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities
{
    public class BigQueryTestStore : TestStore
    {
        //private readonly TestStoreContext _storeContext;
        private const string _projectId = "";
        private const string _datasetId = "";

        public BigQueryTestStore(string name, bool shared) : base(name, shared)
        {
            //Connection = new();
        }

        //todo unhardcode
        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
        => builder.UseBigQuery(_projectId, _datasetId);

        private class TestStoreContext(BigQueryTestStore testStore) : DbContext
        {
            //private readonly BigQueryTestStore _testStore = testStore;

            protected override void OnConfiguring(DbContextOptionsBuilder builder)
            {
                builder.UseBigQuery(_projectId, _datasetId);
            }
        }

        //protected override async Task InitializeAsync(Func<DbContext> createContext, Func<DbContext, Task>? seed, Func<DbContext, Task>? clean)
    }
}