using Microsoft.EntityFrameworkCore.Query;

namespace Ivy.EFCore.BigQuery.Query.Internal
{
    internal class BigQueryQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;

        public BigQueryQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public virtual QuerySqlGenerator Create() => new BigQueryQuerySqlGenerator(_dependencies);
    }
}
