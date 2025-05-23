using AdoNet.Specification.Tests;
using System.Data;
using Xunit;

namespace Ivy.EFCore.BigQuery.Data.Conformance.Tests;

public sealed class DataReaderTests : DataReaderTestBase<SelectValueFixture>
{
    public DataReaderTests(SelectValueFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public override void Read_works()
    {
        using var connection = CreateOpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 UNION ALL SELECT 2;";
        using var reader = command.ExecuteReader();
        var hasData = reader.Read();
        Assert.True(hasData);
        Assert.Equal(1L, reader.GetInt64(0));

        hasData = reader.Read();
        Assert.True(hasData);
        Assert.Equal(2L, reader.GetInt64(0));

        hasData = reader.Read();
        Assert.False(hasData);
    }

    [Fact]
    public override void SingleRow_returns_one_row()
    {
        using var connection = CreateOpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 UNION ALL SELECT 2;";

        using var reader = command.ExecuteReader(CommandBehavior.SingleRow);
        var hasData = reader.Read();
        Assert.True(hasData);
        Assert.Equal(1L, Convert.ToInt64(reader.GetValue(0)));
        Assert.False(reader.Read());
        var x = reader.Read();
        Assert.False(reader.NextResult());
    }

    [Fact(Skip = "BigQueryClient doesn't support multiple result sets")]
    public override void NextResult_works()
    {
    }

    //BQ does not support multiple result sets. Default behavior is to return the last result set.
    [Fact]
    public override void SingleRow_returns_one_result_set()
    {
        using var connection = CreateOpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1; SELECT 2; SELECT 3;";

        using var reader = command.ExecuteReader(CommandBehavior.SingleRow);
        var hasData = reader.Read();
        Assert.True(hasData);
        Assert.Equal(3L, Convert.ToInt64(reader.GetValue(0)));
        Assert.False(reader.Read());

        Assert.False(reader.NextResult());
    }

    [Fact]
    public override void SingleResult_returns_one_result_set()
    {
        using var connection = CreateOpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1; SELECT 2; SELECT 3;";

        using var reader = command.ExecuteReader(CommandBehavior.SingleResult);
        var hasData = reader.Read();
        Assert.True(hasData);
        Assert.Equal(3L, Convert.ToInt64(reader.GetValue(0)));
        Assert.False(reader.Read());

        Assert.False(reader.NextResult());
    }

    [Fact(Skip = "BigQueryClient doesn't return multiple result sets")]
    public override void HasRows_works_when_batching()
    {
    }
}