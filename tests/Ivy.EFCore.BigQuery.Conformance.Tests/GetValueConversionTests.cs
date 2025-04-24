using AdoNet.Specification.Tests;

namespace Ivy.EFCore.BigQuery.Data.Conformance.Tests;

public class GetValueConversionTests : GetValueConversionTestBase<SelectValueFixture>
{
    public GetValueConversionTests(SelectValueFixture fixture)
        : base(fixture)
    {
    }
}