using Google.Cloud.BigQuery.V2;
using Ivy.EFCore.BigQuery.Diagnostics;
using Ivy.EFCore.BigQuery.Infrastructure;
using Ivy.EFCore.BigQuery.Infrastructure.Internal;
using Ivy.EFCore.BigQuery.Migrations;
using Ivy.EFCore.BigQuery.Query.Internal;
using Ivy.EFCore.BigQuery.Storage.Internal;
using Ivy.EFCore.BigQuery.Update.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ivy.EFCore.BigQuery.Extensions
{
    public static class BigQueryServiceCollectionExtensions
    {

        public static IServiceCollection AddBigQuery<TContext>(
       this IServiceCollection serviceCollection,
       string connectionString,
       string databaseName,
       Action<BigQueryDbContextOptionsBuilder>? bigQueryOptionsAction = null,
       Action<DbContextOptionsBuilder>? optionsAction = null)
       where TContext : DbContext
       => serviceCollection.AddDbContext<TContext>(
           (serviceProvider, options) =>
           {
               optionsAction?.Invoke(options);
               options.UseBigQuery(connectionString, bigQueryOptionsAction);
           });

        public static IServiceCollection AddEntityFrameworkBigQuery(this IServiceCollection serviceCollection)
        {

            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)

              .TryAdd<LoggingDefinitions, BigQueryLoggingDefinitions>()
              .TryAdd<IDatabaseProvider, DatabaseProvider<BigQueryOptionsExtension>>()
              .TryAdd<IRelationalTypeMappingSource, BigQueryTypeMappingSource>()
              .TryAdd<ISqlGenerationHelper, BigQuerySqlGenerationHelper>()
              //.TryAdd<IModelValidator, BigQueryModelValidator>() //todo
              .TryAdd<IModificationCommandBatchFactory, BigQueryModificationCommandBatchFactory>()
              .TryAdd<IRelationalDatabaseCreator, BigQueryDatabaseCreator>()
              .TryAdd<IHistoryRepository, BigQueryHistoryRepository>()
              .TryAdd<IRelationalConnection>(p => p.GetRequiredService<IBigQueryRelationalConnection>())
              .TryAdd<IMigrationsSqlGenerator, BigQueryMigrationsSqlGenerator>()
              //.TryAdd<IMemberTranslatorProvider, BigQueryMemberTranslatorProvider>() //todo
              .TryAdd<IMemberTranslatorProvider, RelationalMemberTranslatorProvider>() //todo remove
              .TryAdd<IUpdateSqlGenerator, BigQueryUpdateSqlGenerator>()
              .TryAdd<ISqlExpressionFactory, BigQuerySqlExpressionFactory>()
              .TryAdd<IMethodCallTranslatorProvider, BigQueryMethodCallTranslatorProvider>()
              //https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/method-based-query-syntax-examples-aggregate-operators
              //.TryAdd<IAggregateMethodCallTranslatorProvider, BigQueryAggregateMethodCallTranslatorProvider>() //todo
              .TryAdd<IQuerySqlGeneratorFactory, BigQueryQuerySqlGeneratorFactory>()
              .TryAdd<IExecutionStrategyFactory, BigQueryExecutionStrategyFactory>()
              .TryAdd<IQueryableMethodTranslatingExpressionVisitorFactory, BigQueryQueryableMethodTranslatingExpressionVisitorFactory>()
              .TryAddProviderSpecificServices(
                  s =>
                  {
                      s.TryAddScoped<IBigQueryRelationalConnection, BigQueryRelationalConnection>();
                  })
              .TryAddCoreServices();
            return serviceCollection;
        }
    }
}