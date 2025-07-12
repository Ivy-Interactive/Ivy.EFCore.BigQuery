using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.Storage.Internal
{
    public class BigQueryExecutionStrategyFactory : RelationalExecutionStrategyFactory
    {
        public BigQueryExecutionStrategyFactory(ExecutionStrategyDependencies dependencies) : base(dependencies)
        {
        }

        protected override IExecutionStrategy CreateDefaultStrategy(ExecutionStrategyDependencies dependencies)
        => new BigQueryExecutionStrategy(dependencies);
    }
}