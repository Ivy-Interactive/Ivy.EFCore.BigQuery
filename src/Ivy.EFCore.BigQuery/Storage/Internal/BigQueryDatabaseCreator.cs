using Google;
using Ivy.Data.BigQuery;
using Ivy.EFCore.BigQuery.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ivy.EFCore.BigQuery.Storage.Internal
{
    public class BigQueryDatabaseCreator : RelationalDatabaseCreator
    {
        private readonly IBigQueryRelationalConnection _connection;
        private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;

        public BigQueryDatabaseCreator(
            RelationalDatabaseCreatorDependencies dependencies,
            IBigQueryRelationalConnection connection,
            IRawSqlCommandBuilder rawSqlCommandBuilder)
            : base(dependencies)
        {
            _connection = connection;
            _rawSqlCommandBuilder = rawSqlCommandBuilder;
        }

        public override void Create()
        {
            var datasetId = GetRequiredDatasetId();
            var operations = new[] { new BigQueryCreateDatasetOperation { Name = datasetId } };
            var commands = Dependencies.MigrationsSqlGenerator.Generate(operations);

            Dependencies.MigrationCommandExecutor.ExecuteNonQuery(commands, _connection);
        }

        public override async Task CreateAsync(CancellationToken cancellationToken = default)
        {
            var datasetId = GetRequiredDatasetId();
            var operations = new[] { new BigQueryCreateDatasetOperation() { Name = datasetId } };
            var commands = Dependencies.MigrationsSqlGenerator.Generate(operations);

            await Dependencies.MigrationCommandExecutor.ExecuteNonQueryAsync(commands, _connection, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public override void Delete()
        {
            var datasetId = GetRequiredDatasetId();
            var operations = new[] { new BigQueryDropDatasetOperation { Name = datasetId } };
            var commands = Dependencies.MigrationsSqlGenerator.Generate(operations);

            try
            {
                Dependencies.MigrationCommandExecutor.ExecuteNonQuery(commands, _connection);
            }
            catch (GoogleApiException ex) when (IsDoesNotExist(ex))
            {
            }
        }

        public override async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            var datasetId = GetRequiredDatasetId();
            var operations = new[] { new BigQueryDropDatasetOperation { Name = datasetId } };
            var commands = Dependencies.MigrationsSqlGenerator.Generate(operations);

            try
            {
                await Dependencies.MigrationCommandExecutor.ExecuteNonQueryAsync(commands, _connection, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (GoogleApiException ex) when (IsDoesNotExist(ex))
            {
            }
        }

        public override bool Exists()
        {
            try
            {
                var datasetId = GetRequiredDatasetId();
                var projectId = GetRequiredProjectId();
                var sql = $"SELECT 1 FROM `{projectId}`.`{datasetId}`.INFORMATION_SCHEMA.TABLES LIMIT 1";

                _connection.Open();
                _rawSqlCommandBuilder.Build(sql).ExecuteNonQuery(
                    new RelationalCommandParameterObject(_connection, null, null, null, null));

                return true;
            }
            catch (GoogleApiException ex) when (IsDoesNotExist(ex))
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_connection.DbConnection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        public override async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var datasetId = GetRequiredDatasetId();
                var projectId = GetRequiredProjectId();
                var sql = $"SELECT 1 FROM `{projectId}`.`{datasetId}`.INFORMATION_SCHEMA.TABLES LIMIT 1";

                await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                await _rawSqlCommandBuilder.Build(sql).ExecuteNonQueryAsync(
                    new RelationalCommandParameterObject(_connection, null, null, null, null),
                    cancellationToken).ConfigureAwait(false);

                return true;
            }
            catch (GoogleApiException ex) when (IsDoesNotExist(ex))
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_connection.DbConnection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync().ConfigureAwait(false);
                }
            }
        }

        public override bool HasTables()
        {
            var datasetId = GetRequiredDatasetId();
            var sql = $"SELECT COUNT(1) FROM `{datasetId}`.INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'";

            return (long)_rawSqlCommandBuilder.Build(sql).ExecuteScalar(
                new RelationalCommandParameterObject(_connection, null, null, null, null))! > 0;
        }

        public override async Task<bool> HasTablesAsync(CancellationToken cancellationToken = default)
        {
            var datasetId = GetRequiredDatasetId();
            var sql = $"SELECT COUNT(1) FROM `{datasetId}`.INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'";

            var result = await _rawSqlCommandBuilder.Build(sql).ExecuteScalarAsync(
                new RelationalCommandParameterObject(_connection, null, null, null, null),
                cancellationToken).ConfigureAwait(false);

            return (long)result! > 0;
        }


        private string GetRequiredDatasetId()
        {
            var datasetId = (_connection.DbConnection as BigQueryConnection)?.DefaultDatasetId;
            if (string.IsNullOrEmpty(datasetId))
            {
                throw new InvalidOperationException("A 'DefaultDatasetId' must be specified in the connection string to create or delete the database.");
            }
            return datasetId;
        }

        private string GetRequiredProjectId()
        {
            var projectId = (_connection.DbConnection as BigQueryConnection)?.DefaultProjectId;
            if (string.IsNullOrEmpty(projectId))
            {
                throw new InvalidOperationException(projectId + " must be specified in the connection string to create or delete the database.");
            }
            return projectId;
        }

        private static bool IsDoesNotExist(GoogleApiException exception)
        {
            return exception.HttpStatusCode == System.Net.HttpStatusCode.NotFound;
        }
    }
}
