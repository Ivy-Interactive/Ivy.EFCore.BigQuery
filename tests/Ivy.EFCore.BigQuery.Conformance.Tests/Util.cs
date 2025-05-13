using AdoNet.Specification.Tests;

namespace Ivy.EFCore.BigQuery.Data.Conformance.Tests;

public class Util
{
    public static void ExecuteNonQuery(IDbFactoryFixture factoryFixture, string sql)
    {
        using var connection = factoryFixture.Factory.CreateConnection();
        connection.ConnectionString = Environment.GetEnvironmentVariable("ConnectionString") ?? factoryFixture.ConnectionString;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}