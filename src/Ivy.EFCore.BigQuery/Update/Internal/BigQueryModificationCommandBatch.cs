using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace Ivy.EFCore.BigQuery.Update.Internal
{

    public class BigQueryModificationCommandBatch : AffectedCountModificationCommandBatch
    {
        private const int DefaultMaxBatchSize = 1000;
        private const int MaxParameterCount = 10000;

        private readonly List<IReadOnlyModificationCommand> _pendingBulkInsertCommands = [];

        public BigQueryModificationCommandBatch(
            ModificationCommandBatchFactoryDependencies dependencies)
            : base(dependencies, DefaultMaxBatchSize)
        {
        }

        protected new virtual IBigQueryUpdateSqlGenerator UpdateSqlGenerator
            => (IBigQueryUpdateSqlGenerator)base.UpdateSqlGenerator;


        protected override bool IsValid()
        {
            if (ParameterValues.Count > MaxParameterCount)
            {
                return false;
            }

            return base.IsValid();
        }

        protected override void RollbackLastCommand(IReadOnlyModificationCommand modificationCommand)
        {
            if (_pendingBulkInsertCommands.Count > 0)
            {
                _pendingBulkInsertCommands.RemoveAt(_pendingBulkInsertCommands.Count - 1);
            }

            base.RollbackLastCommand(modificationCommand);
        }

        private void ApplyPendingBulkInsertCommands()
        {
            if (_pendingBulkInsertCommands.Count == 0)
            {
                return;
            }

            var commandPosition = ResultSetMappings.Count;
            var wasCachedCommandTextEmpty = IsCommandTextEmpty;

            var resultSetMapping = UpdateSqlGenerator.AppendBulkInsertOperation(
                SqlBuilder, _pendingBulkInsertCommands, commandPosition, out var requiresTransaction);

            SetRequiresTransaction(!wasCachedCommandTextEmpty || requiresTransaction);

            for (var i = 0; i < _pendingBulkInsertCommands.Count; i++)
            {
                ResultSetMappings.Add(resultSetMapping);
            }

            if (resultSetMapping.HasFlag(ResultSetMapping.HasResultRow))
            {
                ResultSetMappings[^1] &= ~ResultSetMapping.NotLastInResultSet;
                ResultSetMappings[^1] |= ResultSetMapping.LastInResultSet;
            }
        }

        public override bool TryAddCommand(IReadOnlyModificationCommand modificationCommand)
        {
            if (_pendingBulkInsertCommands.Count > 0
                && (modificationCommand.EntityState != EntityState.Added
                    || modificationCommand.StoreStoredProcedure is not null
                    || !CanBeInsertedInSameStatement(_pendingBulkInsertCommands[0], modificationCommand)))
            {
                ApplyPendingBulkInsertCommands();
                _pendingBulkInsertCommands.Clear();
            }

            return base.TryAddCommand(modificationCommand);
        }

        protected override void AddCommand(IReadOnlyModificationCommand modificationCommand)
        {
            if (modificationCommand is { EntityState: EntityState.Added, StoreStoredProcedure: null })
            {
                _pendingBulkInsertCommands.Add(modificationCommand);
                AddParameters(modificationCommand);
            }
            else
            {
                base.AddCommand(modificationCommand);
            }
        }

        private static bool CanBeInsertedInSameStatement(
            IReadOnlyModificationCommand firstCommand,
            IReadOnlyModificationCommand secondCommand)
            => firstCommand.TableName == secondCommand.TableName
                && firstCommand.Schema == secondCommand.Schema
                && firstCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName).SequenceEqual(
                    secondCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName))
                && firstCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName).SequenceEqual(
                    secondCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName));

        public override void Complete(bool moreBatchesExpected)
        {
            ApplyPendingBulkInsertCommands();
            base.Complete(moreBatchesExpected);
        }
    }
}