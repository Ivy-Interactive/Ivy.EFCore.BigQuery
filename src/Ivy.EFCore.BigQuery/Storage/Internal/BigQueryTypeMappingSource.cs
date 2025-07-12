using Google.Cloud.BigQuery.V2;
using Ivy.EFCore.BigQuery.Storage.Internal.Mapping;
using Ivy.EFCore.BigQuery.Storage.ValueConversion.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Concurrent;
using System.Data;

namespace Ivy.EFCore.BigQuery.Storage.Internal
{
    public class BigQueryTypeMappingSource : RelationalTypeMappingSource
    {
        private readonly BigQueryStringTypeMapping _string = new();
        private readonly BigQueryByteArrayTypeMapping _bytes = new();
        private readonly BigQueryBoolTypeMapping _bool = new();
        private readonly BigQueryInt64TypeMapping _long = new();
        private readonly BigQueryDoubleTypeMapping _double = new();
        private readonly BigQueryDateTimeOffsetTypeMapping _timestamp = new();
        private readonly BigQueryDateTimeTypeMapping _dateTime = new();
        private readonly BigQueryDateOnlyTypeMapping _date = new();
        private readonly BigQueryTimeOnlyTypeMapping _time = new();
        private readonly BigQueryDecimalTypeMapping _decimal = new(); // BIGNUMERIC(57, 28)
        private readonly BigQueryNumericTypeMapping _bigNumericDefault = new("BIGNUMERIC");
        private readonly BigQueryGuidTypeMapping _guid = new();

   
        private readonly FloatTypeMapping _float = new("FLOAT64", DbType.Double);
        private readonly IntTypeMapping _int = new("INT64", DbType.Int32);
        private readonly ShortTypeMapping _short = new("INT64", DbType.Int16);
        private readonly ByteTypeMapping _byte = new("INT64", DbType.Byte);

        private readonly ConcurrentDictionary<string, RelationalTypeMapping> _storeTypeMappings;
        private readonly ConcurrentDictionary<Type, RelationalTypeMapping> _clrTypeMappings;

        public BigQueryTypeMappingSource(
            TypeMappingSourceDependencies dependencies,
            RelationalTypeMappingSourceDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
            // --- 2. Populate the lookup dictionaries ---
            // Npgsql uses an array `RelationalTypeMapping[]` to handle cases where one store type
            // can map to multiple CLR types. For BigQuery's simpler system, we can often use a single mapping.
            // For clarity and future-proofing, using a temporary Dictionary<string, List<...>> is good.
            var storeTypeMappings = new Dictionary<string, List<RelationalTypeMapping>>(StringComparer.OrdinalIgnoreCase)
            {
                { "STRING", new List<RelationalTypeMapping> { _string, _guid } }, // Both map to STRING
                { "BYTES", new List<RelationalTypeMapping> { _bytes } },
                { "BOOL", new List<RelationalTypeMapping> { _bool } },
                { "INT64", new List<RelationalTypeMapping> { _long, _int, _short, _byte /*, etc */ } },
                { "INTEGER", new List<RelationalTypeMapping> { _long, _int, _short, _byte /*, etc */ } },
                { "FLOAT64", new List<RelationalTypeMapping> { _double, _float } },
                { "FLOAT", new List<RelationalTypeMapping> { _double, _float } },
                { "TIMESTAMP", new List<RelationalTypeMapping> { _timestamp } },
                { "DATETIME", new List<RelationalTypeMapping> { _dateTime } },
                { "DATE", new List<RelationalTypeMapping> { _date } },
                { "TIME", new List<RelationalTypeMapping> { _time } },
                { "BIGNUMERIC", new List<RelationalTypeMapping> { _bigNumericDefault } },
                { "BIGNUMERIC(57, 28)", new List<RelationalTypeMapping> { _decimal } }
            };

            // Use ConcurrentDictionary for thread safety, as EF Core may use this from multiple threads.
            _storeTypeMappings = new ConcurrentDictionary<string, RelationalTypeMapping>(
                storeTypeMappings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.First()), // Take the first as default
                StringComparer.OrdinalIgnoreCase);

            var clrTypeMappings = new Dictionary<Type, RelationalTypeMapping>
            {
                // Direct maps
                { typeof(string), _string },
                { typeof(byte[]), _bytes },
                { typeof(bool), _bool },
                { typeof(long), _long },
                { typeof(double), _double },
                { typeof(DateTimeOffset), _timestamp },
                { typeof(DateTime), _dateTime },
                { typeof(DateOnly), _date },
                { typeof(TimeOnly), _time },
                { typeof(decimal), _decimal },
                { typeof(Guid), _guid },
                { typeof(BigQueryNumeric), _bigNumericDefault },

                // Indirect maps (which will be handled by ValueConverters implicitly via EF Core's base logic)
                { typeof(float), _float },
                { typeof(int), _int },
                { typeof(short), _short },
                { typeof(byte), _byte },
                // ... etc
            };
            _clrTypeMappings = new ConcurrentDictionary<Type, RelationalTypeMapping>(clrTypeMappings);
        }

        protected override RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            // The Npgsql approach is excellent:
            // 1. Try any registered plugins first (we don't have any, so base.FindMapping is a good stand-in).
            // 2. Try to find a core, built-in mapping.
            // 3. Clone the found mapping with the specific facets from mappingInfo.
            return base.FindMapping(mappingInfo) ?? FindBaseMapping(mappingInfo)?.Clone(mappingInfo);
        }

        protected virtual RelationalTypeMapping? FindBaseMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName;

            if (storeTypeName != null)
            {
                // User specified a store type. Find the best match.
                // In Npgsql, this part is complex because of array/range parsing. For us, it's simpler.
                if (_storeTypeMappings.TryGetValue(storeTypeName, out var mapping))
                {
                    // If a CLR type was also specified, check if this mapping is the one for that CLR type.
                    // If not, we could search the full list in our original dictionary.
                    // For now, we return the default and let EF's logic handle converter composition.
                    return clrType != null && mapping.ClrType != clrType
                        ? FindMapping(clrType) // Re-enter to find the mapping for the CLR type
                        : mapping;
                }

                // Handle parameterized types that aren't in the dictionary, e.g., BIGNUMERIC(30, 10)
                var storeTypeNameBase = GetStoreTypeBaseName(storeTypeName);
                if (_storeTypeMappings.TryGetValue(storeTypeNameBase, out mapping))
                {
                    // Found a base mapping (e.g., for BIGNUMERIC). We can return it.
                    // The `.Clone(mappingInfo)` call in the public FindMapping will apply the precision/scale.
                    return mapping;
                }
            }

            if (clrType != null)
            {
                // User specified a CLR type. This is the most common path.
                if (_clrTypeMappings.TryGetValue(clrType, out var mapping))
                {
                    return mapping;
                }

                // Handle special cases not in the dictionary, e.g., enums.
                if (clrType.IsEnum)
                {
                    // Enums are stored as integers in BigQuery.
                    // We find the mapping for the enum's underlying type (e.g., int, long)
                    // and EF Core's ValueConverter will handle the enum <-> integer conversion.
                    return FindMapping(clrType.GetEnumUnderlyingType());
                }
            }

            return null;
        }

        private string GetStoreTypeBaseName(string storeTypeName)
        {
            var openParen = storeTypeName.IndexOf('(');
            return openParen == -1 ? storeTypeName : storeTypeName.Substring(0, openParen);
        }

        protected override string? ParseStoreTypeName(string? storeTypeName, ref bool? unicode, ref int? size, ref int? precision, ref int? scale)
        {
            if (storeTypeName == null)
            {
                return null;
            }

            var baseName = GetStoreTypeBaseName(storeTypeName);
            var openParen = storeTypeName.IndexOf('(');

            if (openParen > 0)
            {
                var closeParen = storeTypeName.LastIndexOf(')');
                var facets = storeTypeName.Substring(openParen + 1, closeParen - openParen - 1).Split(',');

                if (baseName.Equals("BIGNUMERIC", StringComparison.OrdinalIgnoreCase) ||
                    baseName.Equals("NUMERIC", StringComparison.OrdinalIgnoreCase))
                {
                    if (facets.Length > 0 && int.TryParse(facets[0], out var p))
                    {
                        precision = p;
                    }
                    if (facets.Length > 1 && int.TryParse(facets[1], out var s))
                    {
                        scale = s;
                    }
                }
            }

            return baseName;
        }
    }
}
