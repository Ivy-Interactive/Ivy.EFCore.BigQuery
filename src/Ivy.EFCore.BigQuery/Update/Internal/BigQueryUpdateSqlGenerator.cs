using Microsoft.EntityFrameworkCore.Update;
using System.Text;

namespace Ivy.EFCore.BigQuery.Update.Internal
{
    public class BigQueryUpdateSqlGenerator : UpdateAndSelectSqlGenerator
    {
        //todo implement
        public BigQueryUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies)
        : base(dependencies)
        {}


        protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
        {
            throw new NotImplementedException();
        }

        protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, IColumnModification columnModification)
        {
            throw new NotImplementedException();
        }

        protected override ResultSetMapping AppendSelectAffectedCountCommand(StringBuilder commandStringBuilder, string name, string? schema,
            int commandPosition)
        {
            throw new NotImplementedException();
        }
    }
}
