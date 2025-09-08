using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;

namespace Ivy.EFCore.BigQuery.Storage.Internal.Mapping
{
    public class BigQueryFloatTypeMapping : FloatTypeMapping
    {
        private static readonly Type _clrType = typeof(float);

        public BigQueryFloatTypeMapping()
            : this("FLOAT64")
        {
        }

        protected BigQueryFloatTypeMapping(string storeType)
             : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(_clrType),
                    storeType,
                    StoreTypePostfix.None,
                    System.Data.DbType.Single
                ))
        {
        }

        protected BigQueryFloatTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new BigQueryFloatTypeMapping(parameters);

        protected override string GenerateNonNullSqlLiteral(object value)
            => ((float)value).ToString("R", CultureInfo.InvariantCulture);
    }
}