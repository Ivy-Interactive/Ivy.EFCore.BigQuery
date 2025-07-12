using Ivy.EFCore.BigQuery.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.Migrations
{
    public class BigQueryMigrationsSqlGenerator : MigrationsSqlGenerator
    {

        /// <summary>
        ///     Creates a new <see cref="BigQueryMigrationsSqlGenerator" /> instance.
        /// </summary>
        /// <param name="dependencies">Parameter object containing dependencies for this service.</param>
        public BigQueryMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies) : base(dependencies)
        {
        }

        protected override void Generate(MigrationOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            if (operation is BigQueryCreateDatasetOperation createDatabaseOperation)
            {
                Generate(createDatabaseOperation, model, builder);
                return;
            }

            if (operation is BigQueryDropDatasetOperation dropDatabaseOperation)
            {
                Generate(dropDatabaseOperation, model, builder);
                return;
            }

            base.Generate(operation, model, builder);
        }

        /// <inheritdoc/>
        protected virtual void Generate(BigQueryCreateDatasetOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            builder.Append("CREATE SCHEMA ");
            if (operation.IfNotExists)
            {
                builder.Append("IF NOT EXISTS ");
            }

            if (!string.IsNullOrWhiteSpace(operation.ProjectId))
            {
                builder
                    .Append(DelimitIdentifier(operation.ProjectId))
                    .Append(".");
            }

            builder
                .Append(DelimitIdentifier(operation.Name))
                .EndCommand();
        }

        /// <inheritdoc/>
        protected virtual void Generate(BigQueryDropDatasetOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            builder.Append("DROP SCHEMA ");
            if (operation.IfExists)
            {
                builder.Append("IF EXISTS ");
            }

            if (!string.IsNullOrWhiteSpace(operation.ProjectId))
            {
                builder.Append(DelimitIdentifier(operation.ProjectId)).Append(".");
            }

            builder.Append(DelimitIdentifier(operation.Name));

            if (operation.Behavior == BigQueryDropDatasetOperation.BigQueryDropDatasetBehavior.Cascade)
            {
                builder.Append("CASCADE");
            }
            builder.EndCommand();
        }

        private string DelimitIdentifier(string identifier)
            => Dependencies.SqlGenerationHelper.DelimitIdentifier(identifier);
    }
}
