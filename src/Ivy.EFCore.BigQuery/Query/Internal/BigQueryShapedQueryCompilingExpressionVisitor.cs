using Google.Cloud.BigQuery.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Collections;
using System.Linq.Expressions;
using BigQueryParameter = Ivy.Data.BigQuery.BigQueryParameter;

namespace Ivy.EFCore.BigQuery.Query.Internal
{
    internal sealed class BigQueryShapedQueryCompilingExpressionVisitor : ShapedQueryCompilingExpressionVisitor
    {
        public BigQueryShapedQueryCompilingExpressionVisitor(
            ShapedQueryCompilingExpressionVisitorDependencies dependencies,
            QueryCompilationContext queryCompilationContext)
            : base(dependencies, queryCompilationContext)
        {
        }

        //todo implement
        protected override Expression VisitShapedQuery(ShapedQueryExpression shapedQueryExpression)
        {
            if (shapedQueryExpression.QueryExpression is SelectExpression selectExpression)
            {
                selectExpression.ApplyProjection();

                var shaper = shapedQueryExpression.ShaperExpression;

                var rowParameter = Expression.Parameter(typeof(BigQueryRow), "row");


                var shaperLambda = Expression.Lambda(shaper, QueryCompilationContext.QueryContextParameter, rowParameter);

                return Expression.New(
                typeof(QueryingEnumerable<>).MakeGenericType(shaperLambda.ReturnType).GetConstructors().First(),
                //Expression.Convert(QueryCompilationContext.QueryContextParameter, typeof(BigQueryQueryContext)),
                Expression.Constant(selectExpression),
                Expression.Constant(shaperLambda.Compile()),
                Expression.Constant(
                        QueryCompilationContext.QueryTrackingBehavior == QueryTrackingBehavior.NoTrackingWithIdentityResolution)
                );
            }

            return shapedQueryExpression.QueryExpression;
        }

        //private IEnumerable<BigQueryParameter> ExtractParameters(SelectExpression selectExpression)
        //{
        //    var parameters = new List<BigQueryParameter>();

        //    foreach (var sqlParameter in selectExpression.Parameters)
        //    {
        //        parameters.Add(new BigQueryParameter(sqlParameter.Name, sqlParameter.TypeMapping.DbType)
        //        {
        //            Value = sqlParameter.Value
        //        });
        //    }

        //    return parameters;
        //}

    }


    public class QueryingEnumerable<T> : IEnumerable<T>
    {
        private readonly string _sql;
        private readonly IEnumerable<BigQueryParameter> _parameters;
        private readonly QueryOptions _queryOptions;
        private readonly GetQueryResultsOptions _resultsOptions;
        private readonly BigQueryClient _client;

        public QueryingEnumerable(
        string sql,
        IEnumerable<BigQueryParameter> parameters,
        QueryOptions queryOptions,
        GetQueryResultsOptions resultsOptions,
        BigQueryClient client)
        {
            _sql = sql;
            _parameters = parameters ?? [];
            _queryOptions = queryOptions ?? new QueryOptions();
            _resultsOptions = resultsOptions ?? new GetQueryResultsOptions();
            _client = client;
        }


        public IEnumerator<T> GetEnumerator()
        {
            //var resultSet = _client.ExecuteQuery(_sql, _parameters, _queryOptions, _resultsOptions);
            //foreach (var row in resultSet)
            //{
            //    yield return ConvertRowToEntity<T>(row);
            //} 
            throw new NotImplementedException();

        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        //public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)        

        private T ConvertRowToEntity<T>(BigQueryRow row)
        {
            // Implement conversion logic
            throw new NotImplementedException();
        }
    }
}
