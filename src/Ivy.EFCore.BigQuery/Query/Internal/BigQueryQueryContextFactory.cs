using Microsoft.EntityFrameworkCore.Query;

namespace Ivy.EFCore.BigQuery.Query.Internal
{
    internal class BigQueryQueryContextFactory : IQueryContextFactory
    {
        private readonly QueryContextDependencies _dependencies;
        private readonly IBigQueryClientWrapper _client;

        public BigQueryQueryContextFactory(
            QueryContextDependencies dependencies,
            IBigQueryClientWrapper client
            )
        {
            _dependencies = dependencies;
            _client = client;   
        }

        public QueryContext Create()
        {
            return new BigQueryQueryContext(_dependencies, _client);
        }
    }

    internal sealed class BigQueryQueryContext : QueryContext
    {
        public IBigQueryClientWrapper Client { get; }

        public BigQueryQueryContext(QueryContextDependencies dependencies,
            IBigQueryClientWrapper client) : base(dependencies)
            
        {
            Client = client;
        }
    }
}
