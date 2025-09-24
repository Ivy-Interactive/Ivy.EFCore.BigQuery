﻿using System.Data.Common;
using System.Data;
using Google.Cloud.BigQuery.V2;

namespace Ivy.Data.BigQuery
{
    public class BigQueryParameter : DbParameter
    {
        private string _parameterName = string.Empty;
        private object _value;
        private DbType _dbType = DbType.String;
        private BigQueryDbType? _bqDbType;
        private ParameterDirection _direction = ParameterDirection.Input;
        private bool _isNullable;
        private string _sourceColumn = string.Empty;
        private int _size;

        public BigQueryParameter() { }

        public BigQueryParameter(string parameterName, object value)
        {
            ParameterName = parameterName;
            Value = value;
        }

        public BigQueryParameter(string parameterName, BigQueryDbType bigQueryDbType)
        {
            ParameterName = parameterName;
            BigQueryDbType = bigQueryDbType;
        }

        public BigQueryParameter(string parameterName, BigQueryDbType bigQueryDbType, object value)
        {
            ParameterName = parameterName;
            BigQueryDbType = bigQueryDbType;
            Value = value;
        }

        public BigQueryParameter(string parameterName, DbType dbType)
        {
            ParameterName = parameterName;
            DbType = dbType;
        }

        public BigQueryParameter(string parameterName, DbType dbType, object value)
        {
            ParameterName = parameterName;
            DbType = dbType;
            Value = value;
        }

        public override DbType DbType
        {
            get => _dbType;
            set
            {
                _dbType = value;
                _bqDbType ??= InferBigQueryDbType(value);
            }
        }

        public BigQueryDbType? BigQueryDbType
        {
            get => _bqDbType;
            set
            {
                _bqDbType = value;
                if (value.HasValue)
                {
                    _dbType = InferDbType(value.Value);
                }
            }
        }

        public override ParameterDirection Direction
        {
            get => _direction;
            set
            {
                if (value != ParameterDirection.Input)
                {
                    throw new NotSupportedException("BigQuery only supports Input parameters.");
                }
                _direction = value;
            }
        }

        public sealed override bool IsNullable { get; set; }

        public override string ParameterName
        {
            get => _parameterName;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _parameterName = string.Empty;
                }
                else if (value.StartsWith('@'))
                {
                    _parameterName = value;
                }
                else
                {
                    _parameterName = "@" + value;
                }
            }
        }

        public override string SourceColumn { get => _sourceColumn; set => _sourceColumn = value ?? string.Empty; }
        public override bool SourceColumnNullMapping { get; set; }
        public override DataRowVersion SourceVersion { get; set; } = DataRowVersion.Current;

        public override object Value
        {
            get => _value;
            set
            {
                _value = value;
                if (value != null && !_bqDbType.HasValue)
                {
                    var inferred = InferTypesFromValue(value);
                    _dbType = inferred.DbType;
                    _bqDbType = inferred.BigQueryDbType;
                }
                _isNullable = value == null || value == DBNull.Value;
            }
        }

        public override int Size { get => _size; set => _size = value; }

        public BigQueryParameterCollection? Collection { get; set; }

        public override void ResetDbType()
        {
            _dbType = DbType.String;
            _bqDbType = null;
            Value = null;
        }

        internal static BigQueryDbType? InferBigQueryDbType(DbType dbType) => dbType switch
        {
            DbType.Int64 => Google.Cloud.BigQuery.V2.BigQueryDbType.Int64,
            DbType.Double => Google.Cloud.BigQuery.V2.BigQueryDbType.Float64,
            DbType.Boolean => Google.Cloud.BigQuery.V2.BigQueryDbType.Bool,
            DbType.String => Google.Cloud.BigQuery.V2.BigQueryDbType.String,
            DbType.AnsiString => Google.Cloud.BigQuery.V2.BigQueryDbType.String,
            DbType.StringFixedLength => Google.Cloud.BigQuery.V2.BigQueryDbType.String,
            DbType.AnsiStringFixedLength => Google.Cloud.BigQuery.V2.BigQueryDbType.String,
            DbType.Binary => Google.Cloud.BigQuery.V2.BigQueryDbType.Bytes,
            DbType.DateTime => Google.Cloud.BigQuery.V2.BigQueryDbType.DateTime,
            DbType.DateTimeOffset => Google.Cloud.BigQuery.V2.BigQueryDbType.Timestamp,
            DbType.Date => Google.Cloud.BigQuery.V2.BigQueryDbType.Date,
            DbType.Time => Google.Cloud.BigQuery.V2.BigQueryDbType.Time,
            DbType.Decimal => Google.Cloud.BigQuery.V2.BigQueryDbType.Numeric,
            DbType.VarNumeric => Google.Cloud.BigQuery.V2.BigQueryDbType.Numeric,
            DbType.Guid => Google.Cloud.BigQuery.V2.BigQueryDbType.String,
            DbType.Object => null,
            _ => null,
        };

        internal static DbType InferDbType(BigQueryDbType bqDbType) => bqDbType switch
        {
            Google.Cloud.BigQuery.V2.BigQueryDbType.Int64 => DbType.Int64,
            Google.Cloud.BigQuery.V2.BigQueryDbType.Float64 => DbType.Double,
            Google.Cloud.BigQuery.V2.BigQueryDbType.Bool => DbType.Boolean,
            Google.Cloud.BigQuery.V2.BigQueryDbType.String => DbType.String,
            Google.Cloud.BigQuery.V2.BigQueryDbType.Bytes => DbType.Binary,
            Google.Cloud.BigQuery.V2.BigQueryDbType.Timestamp => DbType.DateTimeOffset,
            Google.Cloud.BigQuery.V2.BigQueryDbType.Date => DbType.Date,
            Google.Cloud.BigQuery.V2.BigQueryDbType.Time => DbType.Time,
            Google.Cloud.BigQuery.V2.BigQueryDbType.DateTime => DbType.DateTime,
            Google.Cloud.BigQuery.V2.BigQueryDbType.Numeric => DbType.Decimal,
            Google.Cloud.BigQuery.V2.BigQueryDbType.BigNumeric => DbType.VarNumeric,
            Google.Cloud.BigQuery.V2.BigQueryDbType.Geography => DbType.String, 
            Google.Cloud.BigQuery.V2.BigQueryDbType.Json => DbType.String, // JSON string
            Google.Cloud.BigQuery.V2.BigQueryDbType.Struct => DbType.Object, 
            Google.Cloud.BigQuery.V2.BigQueryDbType.Array => DbType.Object,
            _ => DbType.Object,
        };

        internal static (DbType DbType, BigQueryDbType? BigQueryDbType) InferTypesFromValue(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return (DbType.Object, null); // Cannot infer from null
            }

            var type = value.GetType();

            if (value is Stream) return (DbType.Binary, Google.Cloud.BigQuery.V2.BigQueryDbType.Bytes);
            if (type == typeof(int) || type == typeof(int?)) return (DbType.Int32, Google.Cloud.BigQuery.V2.BigQueryDbType.Int64);
            if (type == typeof(short) || type == typeof(short?)) return (DbType.Int16, Google.Cloud.BigQuery.V2.BigQueryDbType.Int64);
            if (type == typeof(byte) || type == typeof(byte?)) return (DbType.Byte, Google.Cloud.BigQuery.V2.BigQueryDbType.Int64);
            if (type == typeof(sbyte) || type == typeof(sbyte?)) return (DbType.SByte, Google.Cloud.BigQuery.V2.BigQueryDbType.Int64);
            if (type == typeof(ushort) || type == typeof(ushort?)) return (DbType.UInt16, Google.Cloud.BigQuery.V2.BigQueryDbType.Int64);
            if (type == typeof(uint) || type == typeof(uint?)) return (DbType.UInt32, Google.Cloud.BigQuery.V2.BigQueryDbType.Int64);
            if (type == typeof(ulong) || type == typeof(ulong?)) return (DbType.UInt64, Google.Cloud.BigQuery.V2.BigQueryDbType.BigNumeric);

            if (type == typeof(long) || type == typeof(long?)) return (DbType.Int64, Google.Cloud.BigQuery.V2.BigQueryDbType.Int64);
            if (type == typeof(double) || type == typeof(double?)) return (DbType.Double, Google.Cloud.BigQuery.V2.BigQueryDbType.Float64);
            if (type == typeof(bool) || type == typeof(bool?)) return (DbType.Boolean, Google.Cloud.BigQuery.V2.BigQueryDbType.Bool);
            if (type == typeof(string)) return (DbType.String, Google.Cloud.BigQuery.V2.BigQueryDbType.String);
            if (type == typeof(byte[])) return (DbType.Binary, Google.Cloud.BigQuery.V2.BigQueryDbType.Bytes);
            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?)) return (DbType.DateTimeOffset, Google.Cloud.BigQuery.V2.BigQueryDbType.Timestamp);
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return (DbType.DateTime, Google.Cloud.BigQuery.V2.BigQueryDbType.DateTime);
            }
            if (type == typeof(TimeSpan) || type == typeof(TimeSpan?)) return (DbType.Time, Google.Cloud.BigQuery.V2.BigQueryDbType.Time);
            if (type == typeof(decimal) || type == typeof(decimal?)) return (DbType.Decimal, Google.Cloud.BigQuery.V2.BigQueryDbType.Numeric);
            if (type == typeof(BigQueryNumeric) || type == typeof(BigQueryNumeric?))
            {
                return (DbType.Decimal, Google.Cloud.BigQuery.V2.BigQueryDbType.Numeric);
            }
            if (type == typeof(Guid) || type == typeof(Guid?)) return (DbType.String, Google.Cloud.BigQuery.V2.BigQueryDbType.String);

            if (value is System.Collections.IEnumerable && type != typeof(string) && type != typeof(byte[]))
            {
                return (DbType.Object, null);
            }

            return (DbType.Object, null);
        }

        internal Google.Cloud.BigQuery.V2.BigQueryParameter ToBigQueryParameter()
        {
            if (string.IsNullOrEmpty(ParameterName) || !ParameterName.StartsWith("@"))
            {
                throw new InvalidOperationException("Parameter name must be set and start with '@'.");
            }

            var type = BigQueryDbType ?? InferBigQueryDbType(DbType);

            if (!type.HasValue && Value != null && Value != DBNull.Value)
            {
                type = InferTypesFromValue(Value).BigQueryDbType;
            }

            if (!type.HasValue)
            {
                if (Value is System.Collections.IEnumerable && Value.GetType() != typeof(string) && Value.GetType() != typeof(byte[]))
                {
                    throw new InvalidOperationException($"Cannot automatically determine BigQuery array type for parameter '{ParameterName}'. Set the BigQueryDbType explicitly (e.g., BigQueryDbType.Array with element type).");
                }

                throw new InvalidOperationException($"Cannot determine BigQueryDbType for parameter '{ParameterName}'. Set DbType or BigQueryDbType explicitly.");
            }

            var name = ParameterName[1..];
            var apiValue = Value;

            if (apiValue == DBNull.Value)
            {
                apiValue = null;
            }

            // Handle BigQueryNumeric for BigNumeric parameters - keep as-is
            // The Google Cloud BigQuery library expects BigQueryNumeric for BigNumeric parameters

            else if (apiValue is Stream streamValue)
            {
                byte[] streamBytes;
                if (streamValue.CanSeek)
                {
                    try
                    {
                        streamValue.Position = 0;
                    }
                    catch (NotSupportedException) { }

                    if (streamValue.Length > int.MaxValue)
                    {
                        throw new NotSupportedException("Streams larger than 2GB are not supported for direct parameter binding.");
                    }
                    streamBytes = new byte[streamValue.Length];
                    var bytesRead = 0;
                    var offset = 0;
                    while ((bytesRead = streamValue.Read(streamBytes, offset, streamBytes.Length - offset)) > 0)
                    {
                        offset += bytesRead;
                    }
                    if (offset != streamBytes.Length)
                    {
                        Array.Resize(ref streamBytes, offset);
                    }
                }
                else
                {
                    using var ms = new MemoryStream();
                    streamValue.CopyTo(ms);
                    streamBytes = ms.ToArray();
                }
                apiValue = streamBytes;

                if (type.Value != Google.Cloud.BigQuery.V2.BigQueryDbType.Bytes)
                {
                    type = Google.Cloud.BigQuery.V2.BigQueryDbType.Bytes;
                }
            }

            return new Google.Cloud.BigQuery.V2.BigQueryParameter(name, type.Value, apiValue);
        }
    }
}
