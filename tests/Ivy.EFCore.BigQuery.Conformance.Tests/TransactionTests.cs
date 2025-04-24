using AdoNet.Specification.Tests;
using Xunit;

namespace Ivy.EFCore.BigQuery.Data.Conformance.Tests;

public sealed class TransactionTests : TransactionTestBase<DbFactoryFixture>
{
    public TransactionTests(DbFactoryFixture fixture)
        : base(fixture)
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void BeginTransaction_throws_when_closed()
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void BeginTransaction_throws_when_parallel_transaction()
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void BeginTransaction_works()
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void Commit_transaction_clears_Connection()
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void Commit_transaction_throws_after_Dispose()
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void Commit_transaction_twice_throws()
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void Commit_transaction_then_Rollback_throws()
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void Rollback_transaction_clears_Connection()
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void Rollback_transaction_throws_after_Dispose()
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void Rollback_transaction_twice_throws()
    {
    }

    [Fact(Skip = "BigQuery does not support ADO.NET style transactions")]
    public override void Rollback_transaction_then_Commit_throws()
    {
    }
}