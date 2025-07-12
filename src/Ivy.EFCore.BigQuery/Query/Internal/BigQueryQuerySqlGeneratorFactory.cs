using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.Query.Internal
{
    public class BigQueryQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {

        public BigQueryQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
        {
            Dependencies = dependencies;
        }

        protected virtual QuerySqlGeneratorDependencies Dependencies { get; }

        public virtual QuerySqlGenerator Create()
            => new BigQueryQuerySqlGenerator(Dependencies);
    }
}
