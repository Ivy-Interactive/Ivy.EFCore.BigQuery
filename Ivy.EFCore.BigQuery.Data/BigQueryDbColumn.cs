using Google.Cloud.BigQuery.V2;
using System.Data.Common;

namespace Ivy.EFCore.BigQuery.Data;

public class BigQueryDbColumn : DbColumn
{
    public BigQueryDbColumn(
        string columnName,
        int ordinal,
        Type dataType,
        string dataTypeName,
        bool allowDbNull,
        int? columnSize,
        int? numericPrecision,
        int? numericScale,
        bool isReadOnly = true,
        bool isUnique = false,
        bool isKey = false,
        bool isLong = false,
        string baseSchemaName = null,
        string baseTableName = null
        )
    {
        this.ColumnName = columnName;
        this.ColumnOrdinal = ordinal;
        this.DataType = dataType;
        this.DataTypeName = dataTypeName;
        this.AllowDBNull = allowDbNull;
        this.ColumnSize = columnSize;
        this.NumericPrecision = numericPrecision;
        this.NumericScale = numericScale;
        this.IsReadOnly = isReadOnly;
        this.IsUnique = isUnique;
        this.IsKey = isKey;
        this.IsLong = isLong;
        this.BaseSchemaName = baseSchemaName;
        this.BaseTableName = baseTableName;
        this.BaseColumnName = columnName;

        this.IsAliased = false;
        this.IsExpression = false;
        this.IsHidden = false;
        this.IsIdentity = false;
        this.IsAutoIncrement = false;
        this.UdtAssemblyQualifiedName = null;
        this.BaseCatalogName = null;
        this.BaseServerName = null;

    }
}