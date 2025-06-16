using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Reflection;

namespace Ivy.EFCore.BigQuery.Query.Internal
{
    /// <summary>
    /// Provides translations for LINQ MemberExpression expressions.
    /// </summary>
    internal class BigQueryMemberTranslatorProvider : RelationalMemberTranslatorProvider
    {
        public BigQueryMemberTranslatorProvider(RelationalMemberTranslatorProviderDependencies dependencies) : base(dependencies)
        {
            throw new NotImplementedException();
        }

        //public SqlExpression? Translate(SqlExpression? instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        //{
        //    throw new NotImplementedException();
        //}
    }
}