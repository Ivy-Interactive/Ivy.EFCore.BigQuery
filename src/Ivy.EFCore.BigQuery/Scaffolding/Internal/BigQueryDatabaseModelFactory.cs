using Google.Cloud.BigQuery.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace Ivy.EFCore.BigQuery.Scaffolding.Internal
{
    //todo unhardcode
    public class BigQueryDatabaseModelFactory : DatabaseModelFactory
    {
        private const string _projectId = "";
        private readonly IRelationalTypeMappingSource _typeMappingSource;

        public BigQueryDatabaseModelFactory(
            IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger,
            IRelationalTypeMappingSource typeMappingSource)
        {
            _typeMappingSource = typeMappingSource;
        }


        //todo update
        public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
        {
            var client = BigQueryClient.Create(_projectId);
            var datasetId = "";


            var tables = client.ListTables(_projectId, datasetId);
            var databaseModel = new DatabaseModel();

            foreach (var table in tables)
            {
                var databaseTable = new DatabaseTable { Name = table.Reference.TableId };
                var tableSchema = client.GetTable(table.Reference).Schema;
                DatabaseColumn primaryKeyColumn = null;

                foreach (var field in tableSchema.Fields)
                {
                    Console.WriteLine($"Processing column: {field.Name}, Type: {field.Type}");

                    var column = new DatabaseColumn
                    {
                        Name = field.Name,
                        StoreType = field.Type.ToString().ToLowerInvariant().Replace("geography", "string"),
                        IsNullable = field.Mode != "REQUIRED",
                        Table = databaseTable,

                    };

                    if (primaryKeyColumn == null)
                    {
                        primaryKeyColumn = column;
                    }

                    databaseTable.Columns.Add(column);
                }

                if (primaryKeyColumn != null)
                {
                    var primaryKey = new DatabasePrimaryKey { Table = databaseTable };
                    primaryKey.Columns.Add(primaryKeyColumn);
                    databaseTable.PrimaryKey = primaryKey;
                }

                databaseModel.Tables.Add(databaseTable);
            }

            return databaseModel;
        }

        public override DatabaseModel Create(DbConnection connection, DatabaseModelFactoryOptions options)
        {
            throw new NotImplementedException();
        }

        //https://cloud.google.com/bigquery/docs/information-schema-tables
        private List<DatabaseTable> GetTables(DbConnection connection, IReadOnlyList<string> tableFilters, IReadOnlyList<string> schemaFilters)
        {
            var tables = new List<DatabaseTable>();
            var schema = connection.GetSchema("TABLES");

            foreach (DataRow row in schema.Rows)
            {
                var schemaName = row["TABLE_SCHEMA"] as string;
                var tableName = row["TABLE_NAME"] as string;

                // Apply filters if provided
                if ((schemaFilters.Count == 0 || schemaFilters.Contains(schemaName)) &&
                    (tableFilters.Count == 0 || tableFilters.Contains(tableName)))
                {
                    tables.Add(new DatabaseTable
                    {
                        Schema = schemaName,
                        Name = tableName
                    });
                }
            }
            return tables;
        }

        private List<DatabaseColumn> GetColumns(DbConnection connection, ISet<(string Schema, string Name)> tables)
        {
            var columns = new List<DatabaseColumn>();
            var schema = connection.GetSchema("Columns");

            foreach (DataRow row in schema.Rows)
            {
                var schemaName = row["TABLE_SCHEMA"] as string;
                var tableName = row["TABLE_NAME"] as string;

                if (!tables.Contains((schemaName, tableName)))
                {
                    continue;
                }

                var columnName = row["COLUMN_NAME"] as string;
                var storeType = row["DATA_TYPE"] as string;
                var isNullable = (bool)row["IS_NULLABLE"];
                var ordinal = Convert.ToInt32(row["ORDINAL_POSITION"]);
                var defaultValueSql = row["COLUMN_DEFAULT"] as string;


                //var typeMapping = _typeMappingSource.FindMapping(storeType);
                //if (typeMapping == null)
                //{
                //    Logger.ColumnTypeNotMappedWarning(tableName, columnName, storeType);
                //    continue; 
                //}

                var column = new DatabaseColumn
                {
                    Name = columnName,
                    StoreType = storeType,
                    IsNullable = isNullable,
                    DefaultValueSql = defaultValueSql,
                    ValueGenerated = null, 
                };

                columns.Add(column);
            }
            return columns;
        }
    }
}
