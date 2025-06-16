using Microsoft.EntityFrameworkCore.Storage;
using System.Text;

namespace Ivy.EFCore.BigQuery.Storage.Internal
{
    internal class BigQuerySqlGenerationHelper : RelationalSqlGenerationHelper
    {
        private string _escapeIdentifier(string identifier)
           => identifier.Replace("`", "``");

        public BigQuerySqlGenerationHelper(RelationalSqlGenerationHelperDependencies dependencies) : base(dependencies)
        {            
        }

        // BigQuery uses backticks instead of quotes https://cloud.google.com/bigquery/docs/reference/standard-sql/lexical
        public override string DelimitIdentifier(string name)
            => $"`{name}`";

        //Generates the delimited SQL representation of an identifier(column name, table name, etc.)
        public override string DelimitIdentifier(string? name, string? schema) 
            => schema == null
                ? DelimitIdentifier(name)
                : $"{DelimitIdentifier(schema)}.{DelimitIdentifier(name)}";


        public override void GenerateParameterName(StringBuilder builder, string name)
            => builder.Append('@').Append(name);

        //like GO in TSQL
        public override string BatchTerminator => "";
    }
}
