using Ivy.Data.BigQuery;
using Ivy.EFCore.BigQuery.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities
{
    public class BigQueryTestStore(string name, bool shared = true, string? scriptPath = null)
        : RelationalTestStore(name, shared, CreateConnection(name))
    {
        private static readonly SemaphoreSlim Semaphore = new(1, 1);

        private readonly string _testDatabaseName = name;
        public const int CommandTimeout = 300;

        public static BigQueryTestStore GetOrCreate(
            string name,
            string? scriptPath = null,
            string? additionalSql = null,
            string? connectionStringOptions = null,
            bool useConnectionString = false)
            => new(name, shared: true, scriptPath);

        private static BigQueryConnection CreateConnection(string name)
            => new(CreateConnectionString(name));

        public new BigQueryConnection Connection => (BigQueryConnection)base.Connection;

        public static string CreateConnectionString(string name, string? options = null)
        {
            var builder = new BigQueryConnectionStringBuilder(TestEnvironment.DefaultConnection)
            {
                DefaultDatasetId = name
            };
            return builder.ConnectionString;
        }

        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
            => builder.UseBigQuery(Connection.ConnectionString);

        protected override async Task InitializeAsync(Func<DbContext> createContext, Func<DbContext, Task> seed, Func<DbContext, Task> clean)
        {
            var seedDatabaseName = $"{Name}_seed";

            await Semaphore.WaitAsync();
            try
            {
                await CreateSeedDatabaseOnceAsync(seedDatabaseName, createContext, seed, scriptPath);
            }
            finally
            {
                Semaphore.Release();
            }

            await CopyDatabaseAsync(seedDatabaseName, _testDatabaseName);

            Connection.ConnectionString = CreateConnectionString(_testDatabaseName);
        }

        private static async Task CreateSeedDatabaseOnceAsync(string seedDatabaseName, Func<DbContext> createContext, Func<DbContext, Task>? seed, string scriptPath)
        {
            await using var seedDbContext = createContext();

            seedDbContext.Database.SetConnectionString(CreateConnectionString(seedDatabaseName));

            await seedDbContext.Database.EnsureCreatedAsync();

            if (!string.IsNullOrEmpty(scriptPath))
            {
                var script = await File.ReadAllTextAsync(scriptPath);
                await seedDbContext.Database.ExecuteSqlRawAsync(script);
            }

            if (seed != null)
            {
                await seed(seedDbContext);
            }
        }

        private async Task CopyDatabaseAsync(string sourceDataset, string destinationDataset)
        {
            using var controlConnection = new BigQueryConnection(TestEnvironment.DefaultConnection);
            await controlConnection.OpenAsync();

            using (var dropCmd = controlConnection.CreateCommand())
            {
                dropCmd.CommandText = $"DROP SCHEMA IF EXISTS `{destinationDataset}` CASCADE";
                await dropCmd.ExecuteNonQueryAsync();
            }
            using (var createCmd = controlConnection.CreateCommand())
            {
                createCmd.CommandText = $"CREATE SCHEMA `{destinationDataset}`";
                await createCmd.ExecuteNonQueryAsync();
            }

            var tables = await GetSchemaObjectsAsync(controlConnection, sourceDataset, "BASE TABLE");
            foreach (var table in tables)
            {
                using var copyCmd = controlConnection.CreateCommand();
                copyCmd.CommandText = $"CREATE TABLE `{destinationDataset}`.`{table.Key}` COPY `{sourceDataset}`.`{table.Key}`";
                await copyCmd.ExecuteNonQueryAsync();
            }


            var views = await GetSchemaObjectsAsync(controlConnection, sourceDataset, "VIEW");
            foreach (var view in views)
            {
                using var viewCmd = controlConnection.CreateCommand();

                viewCmd.CommandText = view.Value.Replace($"`{sourceDataset}`", $"`{destinationDataset}`");
                await viewCmd.ExecuteNonQueryAsync();
            }

            var routines = await GetRoutinesAsync(controlConnection, sourceDataset);
            foreach (var routine in routines)
            {
                using var routineCmd = controlConnection.CreateCommand();
                routineCmd.CommandText = routine.Value.Replace($"`{sourceDataset}`", $"`{destinationDataset}`");
                await routineCmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<Dictionary<string, string>> GetSchemaObjectsAsync(DbConnection connection, string dataset, string objectType)
        {
            var objects = new Dictionary<string, string>();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT table_name, ddl FROM `{dataset}`.INFORMATION_SCHEMA.TABLES WHERE table_type = @objectType";
            var param = command.CreateParameter();
            param.ParameterName = "@objectType";
            param.Value = objectType;
            command.Parameters.Add(param);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                objects.Add(reader.GetString(0), reader.IsDBNull(1) ? string.Empty : reader.GetString(1));
            }
            return objects;
        }

        private async Task<Dictionary<string, string>> GetRoutinesAsync(DbConnection connection, string dataset)
        {
            var routines = new Dictionary<string, string>();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT routine_name, ddl FROM `{dataset}`.INFORMATION_SCHEMA.ROUTINES";
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                routines.Add(reader.GetString(0), reader.GetString(1));
            }
            return routines;
        }

        public override async Task CleanAsync(DbContext context)
        {
            await Task.CompletedTask;
        }

        public void ExecuteScript(string scriptPath)
        {
            var script = File.ReadAllText(scriptPath);
            Execute(
                Connection, command =>
                {
                    foreach (var batch in
                             new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMilliseconds(1000.0))
                                 .Split(script).Where(b => !string.IsNullOrEmpty(b)))
                    {
                        command.CommandText = batch;
                        command.ExecuteNonQuery();
                    }

                    return 0;
                }, "");
        }

        private static T Execute<T>(
        DbConnection connection,
        Func<DbCommand, T> execute,
        string sql,
        bool useTransaction = false,
        object[]? parameters = null)
        => ExecuteCommand(connection, execute, sql, useTransaction, parameters);

        private static T ExecuteCommand<T>(
            DbConnection connection,
            Func<DbCommand, T> execute,
            string sql,
            bool useTransaction,
            object[]? parameters)
        {
            if (connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }

            connection.Open();
            try
            {
                using var transaction = useTransaction ? connection.BeginTransaction() : null;

                T result;
                using (var command = CreateCommand(connection, sql, parameters))
                {
                    command.Transaction = transaction;
                    result = execute(command);
                }

                transaction?.Commit();

                return result;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Closed
                    && connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        private static DbCommand CreateCommand(
    DbConnection connection,
    string commandText,
    IReadOnlyList<object>? parameters = null)
        {
            var command = (BigQueryCommand)connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandTimeout = CommandTimeout;

            if (parameters != null)
            {
                for (var i = 0; i < parameters.Count; i++)
                {
                    command.Parameters.AddWithValue("p" + i, parameters[i]);
                }
            }

            return command;
        }

        public override void Dispose()
        {
            using var controlConnection = new BigQueryConnection(TestEnvironment.DefaultConnection);
            controlConnection.Open();
            using var dropCmd = controlConnection.CreateCommand();
            dropCmd.CommandText = $"DROP SCHEMA IF EXISTS `{_testDatabaseName}` CASCADE";
            dropCmd.ExecuteNonQuery();

            base.Dispose();
        }
    }
}