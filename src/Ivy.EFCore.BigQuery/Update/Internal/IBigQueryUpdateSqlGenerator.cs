using Microsoft.EntityFrameworkCore.Update;
using System.Text;

namespace Ivy.EFCore.BigQuery.Update.Internal
{
    public interface IBigQueryUpdateSqlGenerator : IUpdateSqlGenerator
    {
        ResultSetMapping AppendBulkInsertOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
            int commandPosition,
            out bool requiresTransaction);
    }
}
