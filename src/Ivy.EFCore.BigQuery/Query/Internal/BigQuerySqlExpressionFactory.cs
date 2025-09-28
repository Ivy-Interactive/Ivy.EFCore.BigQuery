
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Ivy.EFCore.BigQuery.Query.Internal;

public class BigQuerySqlExpressionFactory : SqlExpressionFactory
{
    private readonly IRelationalTypeMappingSource _typeMappingSource;

    public BigQuerySqlExpressionFactory(SqlExpressionFactoryDependencies dependencies) : base(dependencies)
    {
        _typeMappingSource = dependencies.TypeMappingSource;
    }



    public override SqlExpression Convert(SqlExpression operand, Type type, RelationalTypeMapping? typeMapping)
    {
        if (UnwrapNullableType(type) == typeof(DateTime) && UnwrapNullableType(operand.Type) == typeof(DateTimeOffset))
        {
            return new SqlFunctionExpression(
                "DATETIME",
                new[] { operand },
                nullable: true,
                argumentsPropagateNullability: new[] { true },
                type,
                typeMapping);
        }

        return base.Convert(operand, type, typeMapping);
    }



    //[return: NotNullIfNotNull("sqlExpression")]
    //public override SqlExpression? ApplyTypeMapping(SqlExpression? sqlExpression, RelationalTypeMapping? typeMapping)
    //{
    //    if (sqlExpression is not null && sqlExpression.TypeMapping is null)
    //    {
    //        sqlExpression = sqlExpression switch
    //        {
    //            _ => base.ApplyTypeMapping(sqlExpression, typeMapping)
    //        };
    //    }

    //    return sqlExpression;
    //}

    public static Type UnwrapNullableType(Type type)
        => Nullable.GetUnderlyingType(type) ?? type;

    /// <summary>
    /// Creates a BigQuery array access expression.
    /// </summary>
    public virtual BigQueryArrayAccessExpression ArrayAccess(
        SqlExpression array,
        SqlExpression index,
        bool useOrdinal,
        Type type,
        RelationalTypeMapping? typeMapping = null)
    {
        typeMapping ??= _typeMappingSource.FindMapping(type);
        return new BigQueryArrayAccessExpression(array, index, useOrdinal, type, typeMapping);
    }

    /// <summary>
    /// Creates a BigQuery struct field access expression.
    /// </summary>
    public virtual BigQueryStructAccessExpression StructAccess(
        SqlExpression @struct,
        string fieldName,
        Type type,
        RelationalTypeMapping? typeMapping = null)
    {
        typeMapping ??= _typeMappingSource.FindMapping(type);
        return new BigQueryStructAccessExpression(@struct, fieldName, type, typeMapping);
    }

    /// <summary>
    /// Creates a BigQuery array constructor expression.
    /// </summary>
    public virtual BigQueryArrayConstructorExpression ArrayConstructor(
        IReadOnlyList<SqlExpression> elements,
        Type type,
        RelationalTypeMapping? typeMapping = null,
        bool useArrayKeyword = false,
        string? explicitType = null)
    {
        typeMapping ??= _typeMappingSource.FindMapping(type);
        return new BigQueryArrayConstructorExpression(elements, type, typeMapping, useArrayKeyword, explicitType);
    }

    /// <summary>
    /// Creates a BigQuery struct constructor expression.
    /// </summary>
    public virtual BigQueryStructConstructorExpression StructConstructor(
        IReadOnlyList<SqlExpression> arguments,
        Type type,
        RelationalTypeMapping? typeMapping = null,
        IReadOnlyList<string>? fieldNames = null,
        string? explicitType = null)
    {
        typeMapping ??= _typeMappingSource.FindMapping(type);
        return new BigQueryStructConstructorExpression(arguments, type, typeMapping, fieldNames, explicitType);
    }

    /// <summary>
    /// Creates a BigQuery UNNEST expression.
    /// </summary>
    public virtual BigQueryUnnestExpression Unnest(
        string alias,
        SqlExpression arrayExpression,
        bool withOffset = false,
        bool useOrdinal = false,
        string? offsetAlias = null)
    {
        return new BigQueryUnnestExpression(alias, arrayExpression, withOffset, useOrdinal, offsetAlias);
    }
}
