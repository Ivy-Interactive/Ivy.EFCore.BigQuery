using Google.Cloud.BigQuery.V2;
using Ivy.EFCore.BigQuery.Diagnostics;
using Ivy.EFCore.BigQuery.Infrastructure;
using Ivy.EFCore.BigQuery.Query.Internal;
using Ivy.EFCore.BigQuery.Storage.Internal;
using Ivy.EFCore.BigQuery.Update.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.DependencyInjection;

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
              //.TryAdd<IDatabase, BigQueryDatabase>()
              .TryAdd<IRelationalTypeMappingSource, BigQueryTypeMappingSource>()
              .TryAdd<ISqlGenerationHelper, BigQuerySqlGenerationHelper>()
              .TryAdd<IModificationCommandBatchFactory, BigQueryModificationCommandBatchFactory>()
              //.TryAdd<IRelationalDatabaseCreator, BigQueryDatabaseCreator>()
              .TryAdd<IQueryContextFactory, BigQueryQueryContextFactory>()
              .TryAdd<IRelationalDatabaseCreator, BigQueryDatabaseCreator>()
              .TryAdd<IRelationalConnection>(p => p.GetRequiredService<IBigQueryRelationalConnection>())
              .TryAdd<IMemberTranslatorProvider, BigQueryMemberTranslatorProvider>()
              .TryAdd<IUpdateSqlGenerator, BigQueryUpdateSqlGenerator>()
              .TryAdd<ISqlExpressionFactory, BigQuerySqlExpressionFactory>()
              .TryAdd<IMethodCallTranslatorProvider, BigQueryMethodCallTranslatorProvider>()
              .TryAddProviderSpecificServices(
                  s => s.TryAddScoped<IBigQueryClientWrapper, BigQueryClientWrapper>())
              .TryAddCoreServices();
            return serviceCollection;
        }
    }
}
