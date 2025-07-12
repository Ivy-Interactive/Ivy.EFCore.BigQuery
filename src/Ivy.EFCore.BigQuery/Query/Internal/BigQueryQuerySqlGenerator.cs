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
    public class BigQueryQuerySqlGenerator : QuerySqlGenerator
    {
        public BigQueryQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies) : base(dependencies)
        {
        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            return base.VisitExtension(extensionExpression);
        }

        //https://cloud.google.com/bigquery/docs/reference/standard-sql/query-syntax#select_list
        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            return base.VisitSelect(selectExpression);
        }


        //protected override Expression VisitTable(TableExpression tableExpression)
        //{

        //    Sql.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(tableExpression.Name))
        //       .Append(" AS ")
        //       .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(tableExpression.Alias));


        //    // Sql.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(tableExpression.Schema))
        //    //    .Append('.')
        //    //    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(tableExpression.Name))

        //    return tableExpression;
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