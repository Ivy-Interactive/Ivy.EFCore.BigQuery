using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;

namespace Ivy.EFCore.BigQuery.Storage.Internal.Mapping
{
    public class BigQueryByteTypeMapping : ByteTypeMapping
    {
        private static readonly Type _clrType = typeof(byte);

        public BigQueryByteTypeMapping()
            : this("INT64")
        {
        }

        protected BigQueryByteTypeMapping(string storeType)
             : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(_clrType),
                    storeType,
                    StoreTypePostfix.None,
                    System.Data.DbType.Byte
                ))
        {
        }

        protected BigQueryByteTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new BigQueryByteTypeMapping(parameters);

        protected override string GenerateNonNullSqlLiteral(object value)
            => ((byte)value).ToString(CultureInfo.InvariantCulture);
    }
}