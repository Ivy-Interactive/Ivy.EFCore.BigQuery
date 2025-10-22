using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace Ivy.EFCore.BigQuery.Infrastructure.Internal
{
    public class BigQueryModelValidator : RelationalModelValidator
    {
        public BigQueryModelValidator(ModelValidatorDependencies dependencies, RelationalModelValidatorDependencies relationalDependencies) 
            : base(dependencies, relationalDependencies)
        {
        }

        public override void Validate(IModel model, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            base.Validate(model, logger);

            ValidateBigQueryEntityMappings(model, logger);
        }

        private void ValidateBigQueryEntityMappings(IModel model, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {           
            foreach (var entityType in model.GetEntityTypes())
            {
                if (entityType.FindPrimaryKey() == null || entityType.IsOwned())
                    continue;

                var tableName = entityType.GetTableName();
                var schema = entityType.GetSchema();
                
                if (string.IsNullOrEmpty(tableName))
                {
                    logger.Logger.Log(
                        LogLevel.Warning,
                        new EventId(1, "BigQueryModelValidation"),
                        $"BigQuery entity type '{entityType.DisplayName()}' has no table name configured.",
                        null,
                        (state, ex) => state.ToString()
                    );
                }
            }
        }
    }
}