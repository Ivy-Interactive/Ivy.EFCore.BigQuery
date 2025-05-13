using Google.Cloud.BigQuery.V2;

namespace Ivy.EFCore.BigQuery.Data;

public static class Util
{
    private static readonly Dictionary<BigQueryDbType, string> ParameterApiToDbType = new Dictionary<BigQueryDbType, string>
    {
        { BigQueryDbType.Int64, "INTEGER" },
        { BigQueryDbType.Float64, "FLOAT" },
        { BigQueryDbType.Bool, "BOOL" },
        { BigQueryDbType.String, "STRING" },
        { BigQueryDbType.Bytes, "BYTES" },
        { BigQueryDbType.Date, "DATE" },
        { BigQueryDbType.DateTime, "DATETIME" },
        { BigQueryDbType.Time, "TIME" },
        { BigQueryDbType.Timestamp, "TIMESTAMP" },
        { BigQueryDbType.Array, "ARRAY" },
        { BigQueryDbType.Struct, "STRUCT" },
        { BigQueryDbType.Numeric, "NUMERIC" },
        { BigQueryDbType.Geography, "GEOGRAPHY" },
        { BigQueryDbType.BigNumeric, "BIGNUMERIC" },
        { BigQueryDbType.Json, "JSON" }
    };

    private static readonly Dictionary<string, BigQueryDbType> _nameToTypeMapping = new Dictionary<string, BigQueryDbType>
    {
        { "INTEGER", BigQueryDbType.Int64 },
        { "FLOAT", BigQueryDbType.Float64 },
        { "BOOL", BigQueryDbType.Bool },
        { "STRING", BigQueryDbType.String },
        { "BYTES", BigQueryDbType.Bytes },
        { "DATE", BigQueryDbType.Date },
        { "DATETIME", BigQueryDbType.DateTime },
        { "TIME", BigQueryDbType.Time },
        { "TIMESTAMP", BigQueryDbType.Timestamp },
        { "ARRAY", BigQueryDbType.Array },
        { "STRUCT", BigQueryDbType.Struct },
        { "NUMERIC", BigQueryDbType.Numeric },
        { "GEOGRAPHY", BigQueryDbType.Geography },
        { "BIGNUMERIC", BigQueryDbType.BigNumeric },
        { "JSON", BigQueryDbType.Json }
    };


    public static string DbTypeToParameterApiType(BigQueryDbType type) => ParameterApiToDbType[type];

    public static BigQueryDbType ParameterApiTypeToDbType(string typeName)
    {
        if (_nameToTypeMapping.TryGetValue(typeName, out var type))
        {
            return type;
        }
        throw new ArgumentException($"Unknown BigQuery type: {typeName}");
    }
}