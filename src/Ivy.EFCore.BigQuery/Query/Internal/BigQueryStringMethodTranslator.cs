
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Reflection;

namespace Ivy.EFCore.BigQuery.Query.Internal
{
    public class BigQueryStringMethodTranslator : IMethodCallTranslator
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public BigQueryStringMethodTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public SqlExpression? Translate(SqlExpression? instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (instance == null)
            {
                return null;
            }

            if (method.Name == nameof(string.ToLower) && arguments.Count == 0)
            {
                return _sqlExpressionFactory.Function("LOWER", new[] { instance }, true, new[] { true }, typeof(string), instance.TypeMapping);
            }

            if (method.Name == nameof(string.ToUpper) && arguments.Count == 0)
            {
                return _sqlExpressionFactory.Function("UPPER", new[] { instance }, true, new[] { true }, typeof(string), instance.TypeMapping);
            }

            if (method.Name == nameof(string.StartsWith) && arguments.Count == 1)
            {
                var pattern = arguments[0];
                if (pattern is SqlConstantExpression constantExpression)
                {
                    if (constantExpression.Value is string value)
                    {
                        pattern = _sqlExpressionFactory.Constant(value + "%");
                    }
                }
                else
                {
                    pattern = _sqlExpressionFactory.Function("CONCAT", new[] { pattern, _sqlExpressionFactory.Constant("%") }, true, new[] { true, true }, typeof(string));
                }

                return _sqlExpressionFactory.Like(instance, pattern);
            }

            return null;
        }
    }
}
