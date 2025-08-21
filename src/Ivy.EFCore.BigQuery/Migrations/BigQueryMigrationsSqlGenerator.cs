﻿using Ivy.EFCore.BigQuery.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Ivy.EFCore.BigQuery.Metadata.Internal;

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

        /// <inheritdoc/>
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
        protected override void PrimaryKeyConstraint(
            AddPrimaryKeyOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            builder
                .Append("PRIMARY KEY ");

            IndexTraits(operation, model, builder);

            builder.Append("(")
                .Append(ColumnList(operation.Columns))
                .Append(")");

            IndexOptions(operation, model, builder);

            builder.Append(" NOT ENFORCED");
        }

        /// <inheritdoc/>
        protected override void ColumnDefinition(
            string? schema,
            string table,
            string name,
            ColumnOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            var columnType = operation.ColumnType
                ?? GetColumnType(schema, table, name, operation, model);

            builder
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name))
                .Append(" ")
                .Append(columnType);

            if (operation.IsNullable == false)
            {
                builder.Append(" NOT NULL");
            }

            if (operation.DefaultValueSql != null)
            {
                builder
                    .Append(" DEFAULT ")
                    .Append(operation.DefaultValueSql);
            }
            else if (operation.DefaultValue != null)
            {
                var valueType = operation.DefaultValue.GetType();
                var mapping = (columnType != null
                        ? Dependencies.TypeMappingSource.FindMapping(valueType, columnType)
                        : null)
                    ?? Dependencies.TypeMappingSource.FindMapping(valueType)
                    ?? throw new InvalidOperationException($"No type mapping for default value of type {valueType}");

                builder
                    .Append(" DEFAULT ")
                    .Append(mapping.GenerateSqlLiteral(operation.DefaultValue));
            }
        }

        protected override void UniqueConstraint(
            AddUniqueConstraintOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
            => throw new NotSupportedException("UNIQUE constraints are not supported by BigQuery.");


        protected override void ForeignKeyConstraint(
            AddForeignKeyOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            builder
                .Append("FOREIGN KEY (")
                .Append(ColumnList(operation.Columns))
                .Append(") REFERENCES ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.PrincipalTable, operation.PrincipalSchema))
                .Append(" (")
                .Append(ColumnList(operation.PrincipalColumns))
                .Append(") NOT ENFORCED");
        }

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

        protected override void Generate(EnsureSchemaOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            builder
                .Append("CREATE SCHEMA IF NOT EXISTS ")
                .Append(DelimitIdentifier(operation.Name))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                .EndCommand();
        }

        protected override void Generate(DropSchemaOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            builder
                .Append("DROP SCHEMA IF EXISTS ")
                .Append(DelimitIdentifier(operation.Name))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                .EndCommand();
        }

        // CREATE TABLE
        protected override void Generate(
            CreateTableOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            // Check for BigQuery table annotations
            var createOrReplace = operation[BigQueryAnnotationNames.CreateOrReplace] as bool? == true;
            var ifNotExists = operation[BigQueryAnnotationNames.IfNotExists] as bool? == true;
            var isTempTable = operation[BigQueryAnnotationNames.TempTable] as bool? == true;

            // Validate that OR REPLACE and IF NOT EXISTS are not used together
            if (createOrReplace && ifNotExists)
            {
                throw new InvalidOperationException("CREATE OR REPLACE and IF NOT EXISTS cannot be used together in BigQuery.");
            }

            builder.Append("CREATE");

            if (createOrReplace)
            {
                builder.Append(" OR REPLACE");
            }

            if (isTempTable)
            {
                builder.Append(" TEMP");
            }

            builder.Append(" TABLE");

            if (ifNotExists)
            {
                builder.Append(" IF NOT EXISTS");
            }

            builder
                .Append(" ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
                .AppendLine(" (");

            using (builder.Indent())
            {
                CreateTableColumns(operation, model, builder);
                CreateTableConstraints(operation, model, builder);
                builder.AppendLine();
            }

            builder.Append(")");

            if (terminate)
            {
                builder
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                    .EndCommand();
            }
        }

        protected override void Generate(
            AddColumnOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" ADD COLUMN ");            

            ColumnDefinition(operation.Schema, operation.Table, operation.Name, operation, model, builder);

            if (terminate)
            {
                builder
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                    .EndCommand();
            }
        }

        protected override void Generate(
            DropColumnOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP COLUMN ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name));

            if (terminate)
            {
                builder
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                    .EndCommand();
            }
        }

        protected override void Generate(RenameTableOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            if (!string.IsNullOrEmpty(operation.NewName) && operation.NewName != operation.Name)
            {
                builder
                    .Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema))
                    .Append(" RENAME TO ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName))
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                    .EndCommand();
            }
        }

        protected override void Generate(RenameColumnOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" RENAME COLUMN ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(" TO ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                .EndCommand();
        }

        protected override void Generate(AlterColumnOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            var tableName = Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema);
            var columnName = Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name);
            var old = operation.OldColumn;

            var columnType = operation.ColumnType
                ?? GetColumnType(operation.Schema, operation.Table, operation.Name, operation, model);
            var oldColumnType = old.ColumnType
                ?? GetColumnType(operation.Schema, operation.Table, operation.Name, old, model);

            var madeChanges = false;

            // only less restrictive supported by BQ
            if (!string.Equals(columnType, oldColumnType, StringComparison.OrdinalIgnoreCase))
            {
                builder
                    .Append("ALTER TABLE ")
                    .Append(tableName)
                    .Append(" ALTER COLUMN ")
                    .Append(columnName)
                    .Append(" SET DATA TYPE ")
                    .Append(columnType)
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                    .EndCommand();
                madeChanges = true;
            }

            if (operation.IsNullable == true && old.IsNullable == false)
            {
                builder
                    .Append("ALTER TABLE ")
                    .Append(tableName)
                    .Append(" ALTER COLUMN ")
                    .Append(columnName)
                    .Append(" DROP NOT NULL")
                    .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                    .EndCommand();
                madeChanges = true;
            }

            var defaultChanged = !Equals(operation.DefaultValue, old.DefaultValue) ||
                                 !string.Equals(operation.DefaultValueSql, old.DefaultValueSql, StringComparison.OrdinalIgnoreCase);
            if (defaultChanged)
            {
                if (operation.DefaultValueSql != null)
                {
                    builder
                        .Append("ALTER TABLE ")
                        .Append(tableName)
                        .Append(" ALTER COLUMN ")
                        .Append(columnName)
                        .Append(" SET DEFAULT ")
                        .Append(operation.DefaultValueSql)
                        .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                        .EndCommand();
                    madeChanges = true;
                }
                else if (operation.DefaultValue != null)
                {
                    var valueType = operation.DefaultValue.GetType();
                    var mapping = (columnType != null
                            ? Dependencies.TypeMappingSource.FindMapping(valueType, columnType)
                            : null)
                        ?? Dependencies.TypeMappingSource.FindMapping(valueType)
                        ?? throw new InvalidOperationException($"No type mapping for default value of type {valueType}");

                    builder
                        .Append("ALTER TABLE ")
                        .Append(tableName)
                        .Append(" ALTER COLUMN ")
                        .Append(columnName)
                        .Append(" SET DEFAULT ")
                        .Append(mapping.GenerateSqlLiteral(operation.DefaultValue))
                        .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                        .EndCommand();
                    madeChanges = true;
                }
                else
                {
                    builder
                        .Append("ALTER TABLE ")
                        .Append(tableName)
                        .Append(" ALTER COLUMN ")
                        .Append(columnName)
                        .Append(" DROP DEFAULT")
                        .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                        .EndCommand();
                    madeChanges = true;
                }
            }

            if (!madeChanges)
            {
            }
        }

        protected override void Generate(
            DropPrimaryKeyOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP PRIMARY KEY");

            if (terminate)
            {
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator).EndCommand();
            }
        }

        protected override void Generate(
            DropForeignKeyOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP CONSTRAINT ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name));

            if (terminate)
            {
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator).EndCommand();
            }
        }

        protected override void Generate(
            CreateIndexOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            var isSearchIndex = operation[BigQueryAnnotationNames.SearchIndex] as bool? == true;
            if (!isSearchIndex)
            {
                return;
            }

            var ifNotExists = operation[BigQueryAnnotationNames.IfNotExists] as bool? == true;
            var allColumns = operation[BigQueryAnnotationNames.AllColumns] as bool? == true;

            builder.Append("CREATE SEARCH INDEX ");
            if (ifNotExists)
            {
                builder.Append("IF NOT EXISTS ");
            }

            builder
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(" ON ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append("(");

            if (allColumns)
            {
                builder.Append("ALL COLUMNS");

                // WITH COLUMN OPTIONS for specific columns when ALL COLUMNS is used
                if (operation[BigQueryAnnotationNames.IndexColumnOptions] is IDictionary<string, string> perColumnOptions
                    && perColumnOptions.Count > 0)
                {
                    builder.Append(" WITH COLUMN OPTIONS(");
                    var first = true;
                    foreach (var kvp in perColumnOptions)
                    {
                        if (!first)
                        {
                            builder.Append(", ");
                        }
                        first = false;

                        builder
                            .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(kvp.Key))
                            .Append(" OPTIONS(")
                            .Append(kvp.Value)
                            .Append(")");
                    }
                    builder.Append(")");
                }
            }
            else
            {
                // Explicit column list
                for (var i = 0; i < operation.Columns.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(", ");
                    }
                    var columnName = operation.Columns[i];
                    builder.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(columnName));

                    // Per-column OPTIONS when provided
                    if (operation[BigQueryAnnotationNames.IndexColumnOptions] is IDictionary<string, string> perColumnOptions
                        && perColumnOptions.TryGetValue(columnName, out var opts)
                        && !string.IsNullOrWhiteSpace(opts))
                    {
                        builder
                            .Append(" OPTIONS(")
                            .Append(opts)
                            .Append(")");
                    }
                }
            }

            builder.Append(")");

            // Index OPTIONS
            if (operation[BigQueryAnnotationNames.IndexOptions] is string options && !string.IsNullOrWhiteSpace(options))
            {
                builder
                    .Append(" OPTIONS(")
                    .Append(options)
                    .Append(")");
            }

            if (terminate)
            {
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator).EndCommand();
            }
        }

        protected override void Generate(
            DropIndexOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            var isSearchIndex = operation[BigQueryAnnotationNames.SearchIndex] as bool? == true;
            if (!isSearchIndex)
            {
                // No-op for non-search indexes
                return;
            }

            var ifExists = operation[BigQueryAnnotationNames.IfExists] as bool? == true;

            builder
                .Append("DROP SEARCH INDEX ");
            if (ifExists)
            {
                builder.Append("IF EXISTS ");
            }

            builder
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(" ON ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema));

            if (terminate)
            {
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator).EndCommand();
            }
        }
       
        protected override void Generate(AddCheckConstraintOperation operation, IModel? model, MigrationCommandListBuilder builder)
            => throw new NotSupportedException("CHECK constraints are not supported by BigQuery.");

        protected override void Generate(DropCheckConstraintOperation operation, IModel? model, MigrationCommandListBuilder builder)
            => throw new NotSupportedException("CHECK constraints are not supported by BigQuery.");
    }
}
