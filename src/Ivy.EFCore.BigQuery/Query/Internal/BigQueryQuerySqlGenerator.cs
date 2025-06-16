using Google.Api.Gax;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.Query.Internal
{
    internal class BigQueryQuerySqlGenerator : QuerySqlGenerator
    {
        public BigQueryQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies) : base(dependencies)
        {
        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            return base.VisitExtension(extensionExpression);
        }

        protected override void GenerateTop(SelectExpression selectExpression)
        {
            base.GenerateTop(selectExpression);
        }

        

        //https://cloud.google.com/bigquery/docs/reference/standard-sql/query-syntax#select_list
        //protected override Expression VisitSelect(SelectExpression selectExpression)
        //{
        //    Sql.Append("SELECT ");

        //    if (selectExpression.IsDistinct)
        //    {
        //        Sql.Append("DISTINCT ");
        //    }

        //    // Process Projections
        //    for (int i = 0; i < selectExpression.Projection.Count; i++)
        //    {
        //        if (i > 0)
        //        {
        //            Sql.Append(", ");
        //        }
        //        Visit(selectExpression.Projection[i]);
        //    }

        //    Sql.Append(" FROM ");
        //    Visit(selectExpression.Tables[0]); // Handle FROM clause

        //    // Process WHERE clause
        //    if (selectExpression.Predicate != null)
        //    {
        //        Sql.Append(" WHERE ");
        //        Visit(selectExpression.Predicate);
        //    }

        //    // Process ORDER BY
        //    if (selectExpression.Orderings.Count > 0)
        //    {
        //        Sql.Append(" ORDER BY ");
        //        for (int i = 0; i < selectExpression.Orderings.Count; i++)
        //        {
        //            if (i > 0)
        //            {
        //                Sql.Append(", ");
        //            }
        //            Visit(selectExpression.Orderings[i].Expression);
        //            Sql.Append(selectExpression.Orderings[i].IsAscending ? " ASC" : " DESC");
        //        }
        //    }

        //    return selectExpression;
        //}

        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
            //GaxPreconditions.CheckNotNull(selectExpression, nameof(selectExpression));

            if (selectExpression.Limit != null)
            {
                Sql.Append(" LIMIT ");
                Visit(selectExpression.Limit);
            }

            if (selectExpression.Offset != null)
            {
                Sql.Append(" OFFSET ");
                Visit(selectExpression.Offset);
            }
        }
    }
}