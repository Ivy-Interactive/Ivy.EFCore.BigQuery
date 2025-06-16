using AdoNet.Specification.Tests;

namespace Ivy.Data.BigQuery.Conformance.Tests;

public sealed class ConnectionStringBuilderTests(DbFactoryFixture fixture)
    : ConnectionStringTestBase<DbFactoryFixture>(fixture);