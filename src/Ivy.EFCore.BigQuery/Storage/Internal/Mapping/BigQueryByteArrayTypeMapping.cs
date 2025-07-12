﻿using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivy.EFCore.BigQuery.Storage.Internal.Mapping
{
    public class BigQueryByteArrayTypeMapping : ByteArrayTypeMapping
    {
        public BigQueryByteArrayTypeMapping(string storeType = "BYTES")
             : base(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(byte[])),
                    storeType,
                    StoreTypePostfix.None,
                    System.Data.DbType.Binary
                ))
        {
        }

        protected BigQueryByteArrayTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new BigQueryByteArrayTypeMapping(parameters);
        protected override string GenerateNonNullSqlLiteral(object value)
        {
            var bytes = (byte[])value;
            return $"B'{Convert.ToHexString(bytes)}'";
        }
    }
}
