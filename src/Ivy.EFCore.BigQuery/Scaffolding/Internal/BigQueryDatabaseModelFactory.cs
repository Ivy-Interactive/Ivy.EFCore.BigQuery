using Google.Cloud.BigQuery.V2;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Data.Common;

namespace Ivy.EFCore.BigQuery.Scaffolding.Internal
{
    //todo unhardcode
    public class BigQueryDatabaseModelFactory : DatabaseModelFactory
    {
        private const string _projectId = "";

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
    }
}
