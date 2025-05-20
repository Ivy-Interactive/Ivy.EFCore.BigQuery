using System.Data.Common;
using AdoNet.Specification.Tests;

namespace Ivy.EFCore.BigQuery.Data.Conformance.Tests
{
    public class DbFactoryFixture : IDbFactoryFixture
    {
        public DbFactoryFixture()
        {
            ConnectionString = Environment.GetEnvironmentVariable("BQ_ADO_CONN_STRING", EnvironmentVariableTarget.User) ??
                               "DataSource=http://localhost:9050;AuthMethod=ApplicationDefaultCredentials;ProjectId=test;DefaultDatasetId=ado_tests";
        }

        //public string CreateBooleanLiteral(bool value)
        //{
        //    throw new NotImplementedException();
        //}

        //public string CreateHexLiteral(byte[] value)
        //{
        //    throw new NotImplementedException();
        //}
       // public DbProviderFactory Factory { get; }

        public string ConnectionString { get; } 

        public DbProviderFactory Factory => BigQueryProviderFactory.Instance;
    }
}
