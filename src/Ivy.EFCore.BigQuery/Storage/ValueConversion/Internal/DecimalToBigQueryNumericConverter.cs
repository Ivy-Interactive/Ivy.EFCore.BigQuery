﻿using Google.Cloud.BigQuery.V2;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.Storage.ValueConversion.Internal
{
    public class DecimalToBigQueryNumericConverter : ValueConverter<decimal, BigQueryNumeric>
    {
        public DecimalToBigQueryNumericConverter(ConverterMappingHints? mappingHints = null)
            : base(
                  v => BigQueryNumeric.Parse(v.ToString(CultureInfo.InvariantCulture)),
                  v => v.ToDecimal(LossOfPrecisionHandling.Throw),
                  mappingHints
                  )
        {
        }
    }
}
