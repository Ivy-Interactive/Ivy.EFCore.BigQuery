using AdoNet.Specification.Tests;
using Xunit;

namespace Ivy.EFCore.BigQuery.Data.Conformance.Tests;

public sealed class CommandTests : CommandTestBase<DbFactoryFixture>
{
    public CommandTests(DbFactoryFixture fixture)
        : base(fixture)
    {
    }

    //Different syntax
    [Fact]
    public override void ExecuteScalar_returns_first_when_multiple_rows()
    {
        using var connection = CreateOpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 42 UNION ALL SELECT 43;";
        Assert.Equal(42, Convert.ToInt32(command.ExecuteScalar()));
    }

    [Fact(Skip = "BigQueryClient executes last statement when batching")]
    public override void ExecuteScalar_returns_first_when_batching()
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void ExecuteReader_throws_when_transaction_required()
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void ExecuteReader_throws_when_transaction_mismatched()
    {
    }

    [Fact(Skip = "BigQuery does not support comment statements")]
    public override void ExecuteReader_HasRows_is_false_for_comment()
    {
    }
}