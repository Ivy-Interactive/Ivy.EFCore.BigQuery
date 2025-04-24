using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.Query.Internal
{
    //Todo implement
    public class BigQuerySqlExpressionFactory : SqlExpressionFactory
    {
        private readonly IRelationalTypeMappingSource _typeMappingSource;

        public BigQuerySqlExpressionFactory(SqlExpressionFactoryDependencies dependencies) : base(dependencies)
        => _typeMappingSource = dependencies.TypeMappingSource;

        public SqlExpression Add(SqlExpression left, SqlExpression right, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression And(SqlExpression left, SqlExpression right, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression AndAlso(SqlExpression left, SqlExpression right)
        {
            throw new NotImplementedException();
        }

        [return: NotNullIfNotNull("sqlExpression")]
        public SqlExpression? ApplyDefaultTypeMapping(SqlExpression? sqlExpression)
        {
            throw new NotImplementedException();
        }

        [return: NotNullIfNotNull("sqlExpression")]
        public SqlExpression? ApplyTypeMapping(SqlExpression? sqlExpression, RelationalTypeMapping? typeMapping)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Case(SqlExpression? operand, IReadOnlyList<CaseWhenClause> whenClauses, SqlExpression? elseResult, SqlExpression? existingExpression = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Case(IReadOnlyList<CaseWhenClause> whenClauses, SqlExpression? elseResult)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Coalesce(SqlExpression left, SqlExpression right, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Constant(object value, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Constant(object? value, Type type, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Convert(SqlExpression operand, Type type, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Divide(SqlExpression left, SqlExpression right, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Equal(SqlExpression left, SqlExpression right)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Exists(SelectExpression subquery)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Fragment(string sql)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Function(string name, IEnumerable<SqlExpression> arguments, bool nullable, IEnumerable<bool> argumentsPropagateNullability, Type returnType, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Function(string? schema, string name, IEnumerable<SqlExpression> arguments, bool nullable, IEnumerable<bool> argumentsPropagateNullability, Type returnType, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Function(SqlExpression instance, string name, IEnumerable<SqlExpression> arguments, bool nullable, bool instancePropagatesNullability, IEnumerable<bool> argumentsPropagateNullability, Type returnType, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression GreaterThan(SqlExpression left, SqlExpression right)
        {
            throw new NotImplementedException();
        }

        public SqlExpression GreaterThanOrEqual(SqlExpression left, SqlExpression right)
        {
            throw new NotImplementedException();
        }

        public SqlExpression In(SqlExpression item, SelectExpression subquery)
        {
            throw new NotImplementedException();
        }

        public SqlExpression In(SqlExpression item, IReadOnlyList<SqlExpression> values)
        {
            throw new NotImplementedException();
        }

        public SqlExpression In(SqlExpression item, SqlParameterExpression valuesParameter)
        {
            throw new NotImplementedException();
        }

        public SqlExpression IsNotNull(SqlExpression operand)
        {
            throw new NotImplementedException();
        }

        public SqlExpression IsNull(SqlExpression operand)
        {
            throw new NotImplementedException();
        }

        public SqlExpression LessThan(SqlExpression left, SqlExpression right)
        {
            throw new NotImplementedException();
        }

        public SqlExpression LessThanOrEqual(SqlExpression left, SqlExpression right)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Like(SqlExpression match, SqlExpression pattern, SqlExpression? escapeChar = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression? MakeBinary(ExpressionType operatorType, SqlExpression left, SqlExpression right, RelationalTypeMapping? typeMapping, SqlExpression? existingExpression = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression? MakeUnary(ExpressionType operatorType, SqlExpression operand, Type type, RelationalTypeMapping? typeMapping = null, SqlExpression? existingExpression = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Modulo(SqlExpression left, SqlExpression right, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Multiply(SqlExpression left, SqlExpression right, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Negate(SqlExpression operand)
        {
            throw new NotImplementedException();
        }

        public SqlExpression NiladicFunction(string name, bool nullable, Type returnType, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression NiladicFunction(string schema, string name, bool nullable, Type returnType, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression NiladicFunction(SqlExpression instance, string name, bool nullable, bool instancePropagatesNullability, Type returnType, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Not(SqlExpression operand)
        {
            throw new NotImplementedException();
        }

        public SqlExpression NotEqual(SqlExpression left, SqlExpression right)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Or(SqlExpression left, SqlExpression right, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }

        public SqlExpression OrElse(SqlExpression left, SqlExpression right)
        {
            throw new NotImplementedException();
        }

        public SqlExpression Subtract(SqlExpression left, SqlExpression right, RelationalTypeMapping? typeMapping = null)
        {
            throw new NotImplementedException();
        }
    }
}
