using Microsoft.EntityFrameworkCore.Query;

namespace Ivy.EFCore.BigQuery.Query.Internal;

public class BigQueryMemberTranslatorProvider : RelationalMemberTranslatorProvider
{
    public BigQueryMemberTranslatorProvider(RelationalMemberTranslatorProviderDependencies dependencies)
        : base(dependencies)
    {
        AddTranslators(
        [
            new BigQueryStringMemberTranslator(dependencies.SqlExpressionFactory)
        ]);
    }
}