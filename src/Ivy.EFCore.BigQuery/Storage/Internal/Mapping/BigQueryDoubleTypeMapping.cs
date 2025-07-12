using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.Storage.Internal.Mapping
{
    public class BigQueryDoubleTypeMapping : DoubleTypeMapping
    {
        public BigQueryDoubleTypeMapping(string storeType = "FLOAT64")
            : base(storeType, System.Data.DbType.Double)
        {
        }

        protected BigQueryDoubleTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new BigQueryDoubleTypeMapping(parameters);


        protected override string GenerateNonNullSqlLiteral(object value)
            => ((double)value).ToString("G17", CultureInfo.InvariantCulture);
    }
}
