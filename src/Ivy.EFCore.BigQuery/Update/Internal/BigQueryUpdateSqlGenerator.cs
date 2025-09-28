using Microsoft.EntityFrameworkCore.Update;
using System.Text;

namespace Ivy.EFCore.BigQuery.Update.Internal
{
    public class BigQueryUpdateSqlGenerator : UpdateSqlGenerator
    {
        //todo implement
        public BigQueryUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies)
        : base(dependencies)
        { }

        protected override void AppendUpdateCommand(
        StringBuilder commandStringBuilder,
        string name,
        string? schema,
        IReadOnlyList<IColumnModification> writeOperations,
        IReadOnlyList<IColumnModification> readOperations,
        IReadOnlyList<IColumnModification> conditionOperations,
        bool appendReturningOneClause = false)
        {
            AppendUpdateCommandHeader(commandStringBuilder, name, schema, writeOperations);
            AppendWhereClause(commandStringBuilder, conditionOperations);
            AppendReturningClause(commandStringBuilder, readOperations, appendReturningOneClause ? "1" : null);
            commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);
        }

        protected override void AppendWhereClause(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IColumnModification> operations)
        {
            if (operations.Count == 0)
            {
                commandStringBuilder
                    .AppendLine()
                    .Append("WHERE true");
            }
            else
            {
                base.AppendWhereClause(commandStringBuilder, operations);
            }
        }

    }
}
