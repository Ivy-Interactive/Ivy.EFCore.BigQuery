using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;

namespace Ivy.EFCore.BigQuery.Storage.Internal.Mapping
{
    public class BigQueryShortTypeMapping : ShortTypeMapping
    {
        private static readonly Type _clrType = typeof(short);

        public BigQueryShortTypeMapping()
            : this("INT64")
        {
        }

        protected BigQueryShortTypeMapping(string storeType)
             : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(_clrType),
                    storeType,
                    StoreTypePostfix.None,
                    System.Data.DbType.Int16
                ))
        {
        }

        protected BigQueryShortTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new BigQueryShortTypeMapping(parameters);

        protected override string GenerateNonNullSqlLiteral(object value)
            => ((short)value).ToString(CultureInfo.InvariantCulture);
    }
}