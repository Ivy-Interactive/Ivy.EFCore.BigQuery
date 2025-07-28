using Ivy.Data.BigQuery;
using Ivy.EFCore.BigQuery.Diagnostics;
using Ivy.EFCore.BigQuery.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities;

public class BigQueryTestHelpers : RelationalTestHelpers
{
    protected BigQueryTestHelpers() { }

    public static BigQueryTestHelpers Instance { get; } = new();

    public override IServiceCollection AddProviderServices(IServiceCollection services)
        => services.AddEntityFrameworkBigQuery();

    public override DbContextOptionsBuilder UseProviderOptions(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseBigQuery("DefaultDatasetId=DummyDataset");

    public override LoggingDefinitions LoggingDefinitions { get; } = new BigQueryLoggingDefinitions();
}