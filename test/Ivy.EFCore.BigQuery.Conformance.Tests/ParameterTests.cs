using AdoNet.Specification.Tests;
using Xunit;

namespace Ivy.Data.BigQuery.Conformance.Tests;

public class ParameterTests : ParameterTestBase<DbFactoryFixture>
{
    public ParameterTests(DbFactoryFixture fixture)
        : base(fixture)
    {
    }
}