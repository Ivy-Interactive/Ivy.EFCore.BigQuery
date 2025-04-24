using AdoNet.Specification.Tests;

namespace Ivy.EFCore.BigQuery.Data.Conformance.Tests;

public sealed class ConnectionStringBuilderTests(DbFactoryFixture fixture)
    : ConnectionStringTestBase<DbFactoryFixture>(fixture);