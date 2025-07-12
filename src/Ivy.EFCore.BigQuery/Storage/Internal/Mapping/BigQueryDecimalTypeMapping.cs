using Ivy.Data.BigQuery;
using Ivy.EFCore.BigQuery.Storage.ValueConversion.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.Storage.Internal.Mapping
{
    public class BigQueryDecimalTypeMapping : RelationalTypeMapping
    {

        private static readonly Type _clrType = typeof(decimal);

        public BigQueryDecimalTypeMapping(string storeType = "BIGNUMERIC(57, 28)")
        : base(
            new RelationalTypeMappingParameters(
                new CoreTypeMappingParameters(
                    typeof(decimal),
                    new DecimalToBigQueryNumericConverter()
                    ),
                storeType,
                StoreTypePostfix.PrecisionAndScale,
                System.Data.DbType.Object
            ))
        {
        }


        protected BigQueryDecimalTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
        {
        }


        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new BigQueryDecimalTypeMapping(parameters);


        protected override string GenerateNonNullSqlLiteral(object value)
        {

            var decimalValue = (decimal)value;

            string typePrefix = Parameters.StoreType.StartsWith("BIG", StringComparison.OrdinalIgnoreCase)
                ? "BIGNUMERIC"
                : "NUMERIC";

            return $"{typePrefix} '{decimalValue.ToString(CultureInfo.InvariantCulture)}'";
        }

        protected override void ConfigureParameter(DbParameter parameter)
        {
            base.ConfigureParameter(parameter);

            if (parameter is BigQueryParameter bigQueryParameter)
            {
                if (Parameters.StoreType.StartsWith("BIG", StringComparison.OrdinalIgnoreCase))
                {
                    bigQueryParameter.BigQueryDbType = Google.Cloud.BigQuery.V2.BigQueryDbType.BigNumeric;
                }
                else
                {
                    bigQueryParameter.BigQueryDbType = Google.Cloud.BigQuery.V2.BigQueryDbType.Numeric;
                }
            }
        }
    }
}
