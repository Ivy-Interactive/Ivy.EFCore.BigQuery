using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;

namespace Ivy.EFCore.BigQuery.Query.Internal;

public class BigQueryMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
{
    public BigQueryMethodCallTranslatorProvider(RelationalMethodCallTranslatorProviderDependencies dependencies)
        : base(dependencies)
    {
        var sqlExpressionFactory = (BigQuerySqlExpressionFactory)dependencies.SqlExpressionFactory;
        var typeMappingSource = dependencies.RelationalTypeMappingSource;

        AddTranslators(
        [
            new BigQueryStringMethodTranslator(dependencies.SqlExpressionFactory),
            //new BigQueryArrayMethodTranslator(sqlExpressionFactory, typeMappingSource),
            //new BigQueryStructMethodTranslator(sqlExpressionFactory),
            //new BigQueryArrayFunctionsTranslator(sqlExpressionFactory)
        ]);
    }
}