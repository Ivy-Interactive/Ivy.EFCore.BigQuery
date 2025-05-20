using AdoNet.Specification.Tests;
using Xunit;

namespace Ivy.EFCore.BigQuery.Data.Conformance.Tests;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
public class GetValueConversionTests : GetValueConversionTestBase<SelectValueFixture>
{
    public GetValueConversionTests(SelectValueFixture fixture)
        : base(fixture)
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_maximum_SByte()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_maximum_SByte_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]

    public override async Task GetByte_throws_for_maximum_SByte_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_one_Int16()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_one_Int16_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetByte_throws_for_one_Int16_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_one_Int32()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_one_Int32_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetByte_throws_for_one_Int32_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_one_Int64()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_one_Int64_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetByte_throws_for_one_Int64_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_one_SByte()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_one_SByte_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetByte_throws_for_one_SByte_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_zero_Int16()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_zero_Int16_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetByte_throws_for_zero_Int16_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_zero_Int32()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_zero_Int32_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetByte_throws_for_zero_Int32_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_zero_Int64()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_zero_Int64_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetByte_throws_for_zero_Int64_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_zero_SByte()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetByte_throws_for_zero_SByte_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetByte_throws_for_zero_SByte_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetDecimal_throws_for_minimum_Double()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetDecimal_throws_for_minimum_Double_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetDecimal_throws_for_minimum_Double_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetDecimal_throws_for_minimum_Single()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetDecimal_throws_for_minimum_Single_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetDecimal_throws_for_minimum_Single_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetDecimal_throws_for_one_Double()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetDecimal_throws_for_one_Double_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetDecimal_throws_for_one_Double_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetDecimal_throws_for_one_Single()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetDecimal_throws_for_one_Single_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetDecimal_throws_for_one_Single_with_GetFieldValueAsync()
    {
    }

    //[Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    //public override void GetDecimal_throws_for_one_String()
    //{
    //}

    //[Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    //public override void GetDecimal_throws_for_one_String_with_GetFieldValue()
    //{
    //}

    //[Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    //public override async Task GetDecimal_throws_for_one_String_with_GetFieldValueAsync()
    //{
    //}

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetDecimal_throws_for_zero_Double()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetDecimal_throws_for_zero_Double_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetDecimal_throws_for_zero_Double_with_GetFieldValueAsync()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetDecimal_throws_for_zero_Single()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override void GetDecimal_throws_for_zero_Single_with_GetFieldValue()
    {
    }

    [Fact(Skip = "BigQueryClient doesn't differentiate between different INT64 aliases")]
    public override async Task GetDecimal_throws_for_zero_Single_with_GetFieldValueAsync()
    {
    }

}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously