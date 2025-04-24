using AdoNet.Specification.Tests;
using Xunit;

namespace Ivy.EFCore.BigQuery.Data.Conformance.Tests;

public class ParameterTests : ParameterTestBase<DbFactoryFixture>
{
    public ParameterTests(DbFactoryFixture fixture)
        : base(fixture)
    {
    }
}