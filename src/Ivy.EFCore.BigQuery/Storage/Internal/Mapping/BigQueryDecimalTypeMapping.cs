﻿using Ivy.Data.BigQuery;
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
            string stringValue;
            if (value is decimal decimalValue)
            {
                stringValue = decimalValue.ToString(CultureInfo.InvariantCulture);
            }
            else if (value is Google.Cloud.BigQuery.V2.BigQueryBigNumeric bigQueryBigNumeric)
            {
                stringValue = bigQueryBigNumeric.ToString();
            }
            else if (value is Google.Cloud.BigQuery.V2.BigQueryNumeric bigQueryNumeric)
            {
                stringValue = bigQueryNumeric.ToString();
            }
            else
            {
                stringValue = value.ToString() ?? "0";
            }

            string typePrefix = Parameters.StoreType.StartsWith("BIG", StringComparison.OrdinalIgnoreCase)
                ? "BIGNUMERIC"
                : "NUMERIC";

            return $"{typePrefix} '{stringValue}'";
        }

        protected override void ConfigureParameter(DbParameter parameter)
        {
            base.ConfigureParameter(parameter);

            if (parameter is BigQueryParameter bigQueryParameter)
            {
                // Use Numeric for both NUMERIC and BIGNUMERIC since BigQueryNumeric objects work with Numeric type
                bigQueryParameter.BigQueryDbType = Google.Cloud.BigQuery.V2.BigQueryDbType.Numeric;
            }
        }
    }
}
