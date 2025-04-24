using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
namespace Ivy.EFCore.BigQuery.Query.Internal
{
    //* Date and Time Handling: BigQuery has DATETIME, TIMESTAMP, and DATE types that need special translation.
    //* Array and Struct Support: BigQuery supports ARRAY and STRUCT types, which require special query handling.
    //* Limitations on UPDATE/DELETE: BigQuery has restrictions on UPDATE and DELETE operations, requiring workarounds using MERGE or SELECT INTO.
    //* Window Functions: BigQuery has strong support for analytic functions that might need proper translation.
    //* STRING_AGG and JSON: Some LINQ methods need conversion to BigQuery’s string aggregation or JSON functions.

    internal class BigQueryQueryableMethodTranslatingExpressionVisitor : QueryableMethodTranslatingExpressionVisitor
    {
        private QueryableMethodTranslatingExpressionVisitorDependencies _dependencies;
        private QueryCompilationContext _queryCompilationContext;
        private ISqlExpressionFactory _sqlExpressionFactory;
        private ITypeMappingSource _typeMappingSource;
        private IMemberTranslatorProvider _memberTranslatorProvider;
        private IMethodCallTranslatorProvider _methodCallTranslatorProvider;
        private object _sqlAliasManager;

        public BigQueryQueryableMethodTranslatingExpressionVisitor(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            QueryCompilationContext queryCompilationContext,
            ISqlExpressionFactory sqlExpressionFactory,
            ITypeMappingSource typeMappingSource,
            IMemberTranslatorProvider memberTranslatorProvider,
            IMethodCallTranslatorProvider methodCallTranslatorProvider)
            : base(dependencies, queryCompilationContext, subquery: false)
        {
            _dependencies = dependencies;
            _queryCompilationContext = queryCompilationContext;
            _sqlExpressionFactory = sqlExpressionFactory;
            _typeMappingSource = typeMappingSource;
            _memberTranslatorProvider = memberTranslatorProvider;
            _methodCallTranslatorProvider = methodCallTranslatorProvider;
        }

        protected override ShapedQueryExpression? CreateShapedQueryExpression(IEntityType entityType)
        {
            throw new NotImplementedException();
            //return new ShapedQueryExpression(selectExpression, shaperExpression);
        }

        

        protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateAll(ShapedQueryExpression source, LambdaExpression predicate)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateAny(ShapedQueryExpression source, LambdaExpression? predicate)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateAverage(ShapedQueryExpression source, LambdaExpression? selector, Type resultType)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateCast(ShapedQueryExpression source, Type castType)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateConcat(ShapedQueryExpression source1, ShapedQueryExpression source2)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateContains(ShapedQueryExpression source, Expression item)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateCount(ShapedQueryExpression source, LambdaExpression? predicate)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateDefaultIfEmpty(ShapedQueryExpression source, Expression? defaultValue)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateDistinct(ShapedQueryExpression source)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateElementAtOrDefault(ShapedQueryExpression source, Expression index, bool returnDefault)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateExcept(ShapedQueryExpression source1, ShapedQueryExpression source2)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateFirstOrDefault(ShapedQueryExpression source, LambdaExpression? predicate, Type returnType, bool returnDefault)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateGroupBy(ShapedQueryExpression source, LambdaExpression keySelector, LambdaExpression? elementSelector, LambdaExpression? resultSelector)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateGroupJoin(ShapedQueryExpression outer, ShapedQueryExpression inner, LambdaExpression outerKeySelector, LambdaExpression innerKeySelector, LambdaExpression resultSelector)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateIntersect(ShapedQueryExpression source1, ShapedQueryExpression source2)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateJoin(ShapedQueryExpression outer, ShapedQueryExpression inner, LambdaExpression outerKeySelector, LambdaExpression innerKeySelector, LambdaExpression resultSelector)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateLastOrDefault(ShapedQueryExpression source, LambdaExpression? predicate, Type returnType, bool returnDefault)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateLeftJoin(ShapedQueryExpression outer, ShapedQueryExpression inner, LambdaExpression outerKeySelector, LambdaExpression innerKeySelector, LambdaExpression resultSelector)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateLongCount(ShapedQueryExpression source, LambdaExpression? predicate)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateMax(ShapedQueryExpression source, LambdaExpression? selector, Type resultType)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateMin(ShapedQueryExpression source, LambdaExpression? selector, Type resultType)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateOfType(ShapedQueryExpression source, Type resultType)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateOrderBy(ShapedQueryExpression source, LambdaExpression keySelector, bool ascending)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateReverse(ShapedQueryExpression source)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression TranslateSelect(ShapedQueryExpression source, LambdaExpression selector)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateSelectMany(ShapedQueryExpression source, LambdaExpression collectionSelector, LambdaExpression resultSelector)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateSelectMany(ShapedQueryExpression source, LambdaExpression selector)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateSingleOrDefault(ShapedQueryExpression source, LambdaExpression? predicate, Type returnType, bool returnDefault)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateSkip(ShapedQueryExpression source, Expression count)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateSkipWhile(ShapedQueryExpression source, LambdaExpression predicate)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateSum(ShapedQueryExpression source, LambdaExpression? selector, Type resultType)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateTake(ShapedQueryExpression source, Expression count)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateTakeWhile(ShapedQueryExpression source, LambdaExpression predicate)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateThenBy(ShapedQueryExpression source, LambdaExpression keySelector, bool ascending)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateUnion(ShapedQueryExpression source1, ShapedQueryExpression source2)
        {
            throw new NotImplementedException();
        }

        protected override ShapedQueryExpression? TranslateWhere(ShapedQueryExpression source, LambdaExpression predicate)
        {
            throw new NotImplementedException();
        }


        private SqlExpression? TranslateLambdaExpression(ShapedQueryExpression source, LambdaExpression predicate)
        {
            throw new NotImplementedException();
        }
    }
}