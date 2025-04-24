using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ivy.EFCore.BigQuery.Query.Internal
{
    public class BigQueryQueryableMethodTranslatingExpressionVisitorFactory(
        QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
        ISqlExpressionFactory sqlExpressionFactory,
        ITypeMappingSource typeMappingSource,
        IMemberTranslatorProvider memberTranslatorProvider,
        IMethodCallTranslatorProvider methodCallTranslatorProvider)
        : IQueryableMethodTranslatingExpressionVisitorFactory
    {
        protected virtual QueryableMethodTranslatingExpressionVisitorDependencies Dependencies { get; } = dependencies;

        public virtual QueryableMethodTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)
        => new BigQueryQueryableMethodTranslatingExpressionVisitor(
            dependencies,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
            queryCompilationContext,
            sqlExpressionFactory,
            typeMappingSource,
            memberTranslatorProvider,
            methodCallTranslatorProvider);
    }
}