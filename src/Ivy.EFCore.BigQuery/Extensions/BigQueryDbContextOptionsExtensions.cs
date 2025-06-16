using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Buffers.Text;
using Ivy.EFCore.BigQuery.Infrastructure;

namespace Ivy.EFCore.BigQuery.Extensions
{
    public static class BigQueryDbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder<TContext> UseBigQuery<TContext>(
            this DbContextOptionsBuilder optionsBuilder,
            string projectId,
            string datasetId)
             where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)UseBigQuery(
            (DbContextOptionsBuilder)optionsBuilder, projectId, datasetId);
    

        public static DbContextOptionsBuilder UseBigQuery(
            this DbContextOptionsBuilder optionsBuilder,
            string projectId,
            string datasetId)
            //string credentialsPath)
        {
            var extension = new BigQueryOptionsExtension(projectId, datasetId);
                //, credentialsPath);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            return optionsBuilder;
        }

        //UseBigQuery(connectionString, bigQueryOptionsAction);
        public static DbContextOptionsBuilder UseBigQuery(
       this DbContextOptionsBuilder optionsBuilder,
       string? connectionString,
       Action<BigQueryDbContextOptionsBuilder>? bigQueryOptionsAction = null)
        {
            var extension = (BigQueryOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            ConfigureWarnings(optionsBuilder);

            bigQueryOptionsAction?.Invoke(new BigQueryDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        private static BigQueryOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder options)
        => options.Options.FindExtension<BigQueryOptionsExtension>()
            ?? new BigQueryOptionsExtension();

        private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
        {
            var coreOptionsExtension
                = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
                ?? new CoreOptionsExtension();

            coreOptionsExtension = RelationalOptionsExtension.WithDefaultWarningConfiguration(coreOptionsExtension);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
        }
    }
}
