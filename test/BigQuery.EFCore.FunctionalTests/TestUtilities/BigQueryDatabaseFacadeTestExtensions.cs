using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities;

public static class BigQueryDatabaseFacadeTestExtensions
{
    public static void EnsureClean(this DatabaseFacade databaseFacade)
       => new BigQueryDatabaseCleaner().Clean(databaseFacade);
}