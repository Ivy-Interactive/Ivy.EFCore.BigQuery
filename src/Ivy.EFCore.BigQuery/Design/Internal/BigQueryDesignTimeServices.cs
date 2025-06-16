using Ivy.EFCore.BigQuery.Extensions;
using Ivy.EFCore.BigQuery.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;

[assembly: DesignTimeProviderServices("BigQuery.EFCore.Design.Internal.BigQueryDesignTimeServices")]

namespace Ivy.EFCore.BigQuery.Design.Internal
{
    public class BigQueryDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkBigQuery();
            new EntityFrameworkRelationalDesignServicesBuilder(serviceCollection)
            //serviceCollection.AddEntityFrameworkRelational();
           .TryAdd<IDatabaseModelFactory, BigQueryDatabaseModelFactory>()
           .TryAdd<IProviderConfigurationCodeGenerator, BigQueryConfigurationCodeGenerator>()
           .TryAddCoreServices()
           ;
        }
    }
}
