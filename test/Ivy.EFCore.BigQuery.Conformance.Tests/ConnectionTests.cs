using AdoNet.Specification.Tests;
using Xunit;

namespace Ivy.Data.BigQuery.Conformance.Tests;

public sealed class ConnectionTests : ConnectionTestBase<DbFactoryFixture>
{
    public ConnectionTests(DbFactoryFixture fixture)
        : base(fixture)
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void CreateCommand_does_not_set_Transaction_property()
    {
    }

    [Fact(Skip = "BigQuery doesn't have server versions")]
    public override void ServerVersion_throws_when_closed()
    {
    }
}