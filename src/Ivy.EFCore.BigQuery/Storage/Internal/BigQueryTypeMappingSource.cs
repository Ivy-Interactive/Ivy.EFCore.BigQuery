using Microsoft.EntityFrameworkCore.Storage;

namespace Ivy.EFCore.BigQuery.Storage.Internal
{
    public class BigQueryTypeMappingSource : RelationalTypeMappingSource
    {
        public BigQueryTypeMappingSource(
        TypeMappingSourceDependencies dependencies,
        RelationalTypeMappingSourceDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
        {
        }

        protected override RelationalTypeMapping? FindMapping(in TypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            
            //var storeTypeName = mappingInfo.StoreTypeName;

            //if (clrType == null && storeTypeName != null)
            //{
            //    switch (storeTypeName.ToUpper())
            //    {
            //        case "BIGINT":
            //            return new LongTypeMapping("INT64"); 
            //        case "VARCHAR":
            //        case "NVARCHAR(MAX)":
            //        case "STRING":
            //            return new StringTypeMapping("STRING", System.Data.DbType.String);
            //        case "INT":
            //        case "INTEGER":
            //            return new IntTypeMapping("INT64");
            //        case "FLOAT":
            //            return new FloatTypeMapping("FLOAT64");
            //        case "DOUBLE":
            //            return new DoubleTypeMapping("FLOAT64");
            //        case "BOOL":
            //        case "BOOLEAN":
            //            return new BoolTypeMapping("BOOL");
            //        case "DATETIME":
            //        case "DATETIME2":
            //        case "TIMESTAMP":
            //            return new DateTimeTypeMapping("TIMESTAMP");
            //        case "DECIMAL":
            //        case "NUMERIC":
            //            return new DecimalTypeMapping("NUMERIC");
            //        default:
            //            throw new NotSupportedException($"Unsupported StoreTypeName: {storeTypeName}");
            //    }
            //}

            if (clrType == typeof(string))
            {
                return new StringTypeMapping("STRING", System.Data.DbType.String);
            }
            else if (clrType == typeof(int))
            {
                return new IntTypeMapping("INT64");
            }
            else if (clrType == typeof(long))
            {
                return new LongTypeMapping("INT64");
            }
            else if (clrType == typeof(float))
            {
                return new FloatTypeMapping("FLOAT64");
            }
            else if (clrType == typeof(double))
            {
                return new DoubleTypeMapping("FLOAT64");
            }
            else if (clrType == typeof(bool))
            {
                return new BoolTypeMapping("BOOL");
            }
            else if (clrType == typeof(DateTime))
            {
                return new DateTimeTypeMapping("TIMESTAMP");
            }
            else if (clrType == typeof(decimal))
            {
                return new DecimalTypeMapping("NUMERIC");
            }

            // Handle other types or throw an exception for unsupported types
            if (clrType == null) //&& storeTypeName == null)
            {
                throw new InvalidOperationException("Unable to determine the type mapping. Both ClrType and StoreTypeName are null.");
            }

            Console.WriteLine($"ClrType: {clrType} | StoreTypeName {{storeTypeName}}");


            throw new NotSupportedException($"Unsupported type: {mappingInfo}");
        }
    }
}
