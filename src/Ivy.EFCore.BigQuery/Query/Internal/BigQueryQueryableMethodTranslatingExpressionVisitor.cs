using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
namespace Ivy.EFCore.BigQuery.Query.Internal
{
    // todo
    // Translates LINQ queryable methods (Where, OrderBy, Select) into EF Core's SQL expression tree representation
    public class BigQueryQueryableMethodTranslatingExpressionVisitor : RelationalQueryableMethodTranslatingExpressionVisitor
    {
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public BigQueryQueryableMethodTranslatingExpressionVisitor(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
            RelationalQueryCompilationContext queryCompilationContext)
            : base(dependencies, relationalDependencies, queryCompilationContext)
        {
            _typeMappingSource = relationalDependencies.TypeMappingSource;

            _sqlExpressionFactory = relationalDependencies.SqlExpressionFactory;
        }

        protected BigQueryQueryableMethodTranslatingExpressionVisitor(
            BigQueryQueryableMethodTranslatingExpressionVisitor parentVisitor)
            : base(parentVisitor)
        {
            _typeMappingSource = parentVisitor._typeMappingSource;
            _sqlExpressionFactory = parentVisitor._sqlExpressionFactory;
        }

        protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
            => new BigQueryQueryableMethodTranslatingExpressionVisitor(this);


        protected override ShapedQueryExpression? TranslateOrderBy(
            ShapedQueryExpression source,
            LambdaExpression keySelector,
            bool ascending)
        {
            var translation = base.TranslateOrderBy(source, keySelector, ascending);
            if (translation == null)
            {
                return null;
            }

            return translation;
        }

        protected override ShapedQueryExpression? TranslateCount(ShapedQueryExpression source, LambdaExpression? predicate)
        {
            return base.TranslateCount(source, predicate);
        }
    }
}