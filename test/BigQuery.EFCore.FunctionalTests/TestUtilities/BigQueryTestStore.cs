using Ivy.Data.BigQuery;
using Ivy.EFCore.BigQuery.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using System;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities
{
    public class BigQueryTestStore : RelationalTestStore
    {
        private string _northwindSeedScriptPath = "Northwind.sql";
        private string _northwindRefreshScriptPath = "Northwind.Refresh.sql";

        public static BigQueryTestStore GetOrCreate(string storeName)
            => new BigQueryTestStore(storeName);

        internal static TestStore GetOrCreateWithScriptPath(string storeName, string? seedScriptPath = null, string? refreshScriptPath = null)
            => new BigQueryTestStore(storeName);

        public static async Task<BigQueryTestStore> CreateInitializedAsync(string name)
        {
            var testStore = new BigQueryTestStore(name, shared: false);
            await testStore.InitializeAsync(null, (Func<DbContext>?)null, null);
            return testStore;
        }

        public BigQueryTestStore(string name, bool shared = true, string seedScriptPath = null, string refreshScriptPath = null)
            : base(name, shared, CreateConnection(name))
        {
        }

        private static BigQueryConnection CreateConnection(string name)
            => new(CreateConnectionString(name));

        public new BigQueryConnection Connection => (BigQueryConnection)base.Connection;

        public static string CreateConnectionString(string name, string? options = null)
        {
            var builder = new BigQueryConnectionStringBuilder(TestEnvironment.DefaultConnection);

            //if (options is not null)
            //{
            //    builder.Options = options;
            //}

            return builder.ConnectionString;
        }

        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
            => builder.UseBigQuery(ConnectionString);

        protected override async Task InitializeAsync(Func<DbContext> createContext, Func<DbContext, Task> seed, Func<DbContext, Task> clean)
        {
            using var context = createContext();

            if (await context.Database.EnsureCreatedAsync())
            {
                var script = await File.ReadAllTextAsync(_northwindSeedScriptPath);
                await context.Database.ExecuteSqlRawAsync(script);
            }

            await CleanAsync(context);
        }

        public override async Task CleanAsync(DbContext context)
        {
            await Connection.OpenAsync();
            var tables = Connection.GetSchema("Tables");
            var commands = new StringBuilder();

            foreach (System.Data.DataRow row in tables.Rows)
            {
                var tableId = (string)row["TABLE_NAME"];
                if (tableId.EndsWith("_seed"))
                {
                    var liveTableName = tableId.Replace("_seed", "");
                    commands.AppendLine($"``{Name}`.`{liveTableName}`");
                }
            }

            if (commands.Length > 0)
            {
                var cleanupSql = new StringBuilder();
                foreach (System.Data.DataRow row in tables.Rows)
                {
                    var tableId = (string)row["TABLE_NAME"];
                    if (tableId.EndsWith("_seed"))
                    {
                        var liveTableName = tableId.Replace("_seed", "");
                        cleanupSql.AppendLine($"DROP TABLE IF EXISTS `{Name}.{liveTableName}`;");
                        cleanupSql.AppendLine($"CREATE TABLE `{Name}.{liveTableName}` COPY `{Name}.{tableId}`;");
                    }
                }
                await context.Database.ExecuteSqlRawAsync(cleanupSql.ToString());
            }
        }


    }
}