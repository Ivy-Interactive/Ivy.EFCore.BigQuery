using Ivy.EFCore.BigQuery.Extensions;
using Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities;
using Ivy.EFCore.BigQuery.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Ivy.EFCore.BigQuery.FunctionalTests.Migrations
{
    public class BigQueryMigrationsSqlGeneratorTest() : MigrationsSqlGeneratorTestBase(
       BigQueryTestHelpers.Instance,
        new ServiceCollection(),
        BigQueryTestHelpers.Instance.AddProviderOptions(
            ((IRelationalDbContextOptionsBuilderInfrastructure)
                new BigQueryDbContextOptionsBuilder(new DbContextOptionsBuilder()))
            .OptionsBuilder).Options)
    {
        protected override string GetGeometryCollectionStoreType()
        {
            throw new NotImplementedException();
        }
    }
}
