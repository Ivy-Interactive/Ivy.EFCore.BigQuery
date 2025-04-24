using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace Ivy.EFCore.BigQuery.Storage.Internal
{
    public class BigQueryRelationalConnection : RelationalConnection, IBigQueryRelationalConnection
    {
        public BigQueryRelationalConnection(RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
        }
        public IBigQueryRelationalConnection CreateMasterConnection()
        {
            throw new NotImplementedException();
        }

        protected override DbConnection CreateDbConnection()
        {
            throw new NotImplementedException();
        }
    }
}
