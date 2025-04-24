using AdoNet.Specification.Tests;
using Xunit;

namespace Ivy.EFCore.BigQuery.Data.Conformance.Tests;

public sealed class DataReaderTests : DataReaderTestBase<SelectValueFixture>
{
    public DataReaderTests(SelectValueFixture fixture)
        : base(fixture)
    {
    }
}