using Ivy.EFCore.BigQuery.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities;

public static class BigQueryDbContextOptionsBuilderExtensions
{
    public static BigQueryDbContextOptionsBuilder ApplyConfiguration(this BigQueryDbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);

        optionsBuilder.CommandTimeout(BigQueryTestStore.CommandTimeout);

        return optionsBuilder;
    }
}