using Castle.Core.Logging;
using Ivy.EFCore.BigQuery.Extensions;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities
{
    //public class BigQueryTestStoreFactory : ITestStoreFactory
    //{
    //    public static BigQueryTestStoreFactory Instance { get; } = new();


    //    protected BigQueryTestStoreFactory()
    //    {
    //    }

    //    public IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
    //        => serviceCollection
    //        .AddEntityFrameworkBigQuery()
    //        //.AddSingleton<ILoggerFactory>(new TestSqlLoggerFactory())
    //        ;

    //    public TestStore Create(string storeName) => {};// BigQueryTestStore.Create(storeName);

    //    public ListLoggerFactory CreateListLoggerFactory(Func<string, bool> shouldLogCategory)
    //    => new TestSqlLoggerFactory(shouldLogCategory);
    //    //Todo should I make a new sqlloggerfactory? https://github.com/dotnet/efcore/blob/main/test/EFCore.Cosmos.FunctionalTests/TestUtilities/TestSqlLoggerFactory.cs#L14

    //    public TestStore GetOrCreate(string storeName)
    //     => BigQueryTestStore.GetOrCreate(storeName);
    //}
}
