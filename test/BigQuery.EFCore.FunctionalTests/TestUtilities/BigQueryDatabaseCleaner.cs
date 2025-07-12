using System.Data.Common;
using System.Text;
using Ivy.Data.BigQuery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities
{
    public class BigQueryDatabaseCleaner
    {
        public virtual void Clean(DatabaseFacade database)
        {
            var connection = (BigQueryConnection)database.GetDbConnection();
            var commands = new StringBuilder();

            connection.Open();
            var tables = connection.GetSchema("Tables");

            foreach (System.Data.DataRow row in tables.Rows)
            {
                var tableName = (string)row["TABLE_NAME"];
                if (!tableName.EndsWith("_seed"))
                {
                    commands.AppendLine($"DROP TABLE IF EXISTS `{connection.DefaultDatasetId}.{tableName}`;");
                }
            }

            if (commands.Length > 0)
            {
                database.ExecuteSqlRaw(commands.ToString());
            }
            
            connection.Close();
        }
    }
}
