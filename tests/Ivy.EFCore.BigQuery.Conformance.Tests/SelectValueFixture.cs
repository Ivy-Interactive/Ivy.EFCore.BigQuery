using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using AdoNet.Specification.Tests;

namespace Ivy.EFCore.BigQuery.Data.Conformance.Tests;

public class SelectValueFixture : DbFactoryFixture, ISelectValueFixture, IDeleteFixture
{
    private const string DefaultDatasetId = "ado_tests";
    private const string TableName = "select_value";

    public SelectValueFixture() => CreateSelectValueTable(this);

    public string CreateSelectSql(DbType dbType, ValueKind kind)
    {
        return $"SELECT `{dbType.ToString()}` from {DefaultDatasetId}.{TableName} WHERE Id = {(int)kind};";
    }

    public string CreateSelectSql(byte[] value)
    {
        return $"SELECT FROM_BASE64('{Convert.ToBase64String(value)}') AS Value";
    }

    public string SelectNoRows => $"SELECT * FROM `{DefaultDatasetId}.{TableName}` WHERE false;";

    public string DeleteNoRows => $"DELETE FROM {DefaultDatasetId}.{TableName} WHERE false;";

    public Type NullValueExceptionType => typeof(InvalidCastException);

    public IReadOnlyCollection<DbType> SupportedDbTypes { get; } = new ReadOnlyCollection<DbType>([
        DbType.Int64,
        DbType.Binary,  //BYTES
        DbType.Boolean, //BOOL
        DbType.Byte,    // INT64
        DbType.SByte,   // INT64
        DbType.Int16,   // INT64
        //DbType.UInt16,   // INT64
        DbType.Int32,   // INT64
        DbType.Int64,   // INT64
        //DbType.UInt16,   // INT64
        
        DbType.Double,
        DbType.Single, // FLOAT64
        
        DbType.String,
        DbType.AnsiString,
        DbType.StringFixedLength,
        DbType.AnsiStringFixedLength,
        DbType.Binary,
        DbType.Guid, // Tested as STRING
        DbType.Date,
        DbType.Time,
        DbType.DateTime,
        DbType.DateTime2, // DATETIME
        //DbType.DateTimeOffset, //Todo TIMESTAMP UTC
        DbType.Decimal, // NUMERIC
        DbType.VarNumeric // BIGNUMERIC
    ]);

    public void DropSelectValueTable(IDbFactoryFixture factoryFixture) => Util.ExecuteNonQuery(factoryFixture, $"DROP TABLE IF EXISTS `{DefaultDatasetId}.{TableName}`;");

    public void CreateSelectValueTable(IDbFactoryFixture factoryFixture)
    {
        DropSelectValueTable(factoryFixture);

        Util.ExecuteNonQuery(factoryFixture, $"""

                                              CREATE TABLE `{DefaultDatasetId}.{TableName}` (
                                                Id INT64 NOT NULL,
                                                `Binary` BYTES,
                                                Boolean BOOL,
                                                Byte INT64,
                                                SByte INT64,
                                                Int16 INT64,
                                                UInt16 INT64,
                                                Int32 INT64,
                                                UInt32 INT64,
                                                Int64 INT64,
                                                UInt64 BIGNUMERIC,
                                                Single FLOAT64,
                                                `Double` FLOAT64,
                                                `Decimal` NUMERIC,
                                                String STRING,
                                                Guid STRING,
                                                `Date` DATE,
                                                `DateTime` DATETIME,
                                                `Time` TIME
                                              );

                                              """);

        Util.ExecuteNonQuery(factoryFixture, $"""

                                              INSERT INTO `{DefaultDatasetId}.{TableName}` (
                                                Id, `Binary`, Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64,
                                                Single, `Double`, `Decimal`, String, Guid, `Date`, `DateTime`, `Time`
                                              )
                                              VALUES
                                                (0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
                                                (1, b'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '', '', NULL, NULL, NULL),
                                                (2, b'\x00', FALSE, 0, 0, 0, 0, 0, 0, 0, 0, 0.0, 0.0, 0.0000000000000000000000000000, '0', '00000000-0000-0000-0000-000000000000', NULL, NULL, TIME '00:00:00'),
                                                (3, b'\x11', TRUE, 1, 1, 1, 1, 1, 1, 1, 1, 1.0, 1.0, 0.0000000000000000000000000001, '1', '11111111-1111-1111-1111-111111111111', DATE '1111-11-11', DATETIME '1111-11-11 11:11:11.111', TIME '11:11:11.111'),
                                                (4, NULL, FALSE, 0, -128, -32768, 0, -2147483648, 0, -9223372036854775808, 0, 1.18e-38, 2.23e-308, 0.000000000000001, NULL, '33221100-5544-7766-9988-aabbccddeeff', DATE '1000-01-01', DATETIME '1000-01-01 00:00:00', NULL),
                                                (5, NULL, TRUE, 255, 127, 32767, 65535, 2147483647, 4294967295, 9223372036854775807, BIGNUMERIC '18446744073709551615', 3.40e+38, 1.79e+308, 99999999999999999999.999999999999999, NULL, 'ccddeeff-aabb-8899-7766-554433221100', DATE '9999-12-31', DATETIME '9999-12-31 23:59:59.999', TIME '23:59:59');        

                                              """);
    }

    private string GetLiteral(DbType dbType, ValueKind kind)
    {
        // Null is handled by CAST in CreateSelectSql
        if (kind == ValueKind.Null) return "NULL";

        switch (dbType)
        {
            // Integers (map to INT64 literals)
            case DbType.Int64:
            case DbType.Int32:
            case DbType.Int16:
            case DbType.Byte:
                long longValue = kind switch
                {
                    ValueKind.Zero => 0L,
                    ValueKind.One => 1L,
                    // BQ INT64 range
                    ValueKind.Minimum => -9223372036854775808L, // long.MinValue
                    ValueKind.Maximum => 9223372036854775807L, // long.MaxValue
                    ValueKind.Empty => 0L, // Treat Empty as Zero for integers
                    _ => throw new ArgumentOutOfRangeException(nameof(kind)),
                };
                return longValue.ToString(CultureInfo.InvariantCulture);

            // Floating Point (map to FLOAT64 literals)
            case DbType.Double:
            case DbType.Single:
                double doubleValue = kind switch
                {
                    ValueKind.Zero => 0.0,
                    ValueKind.One => 1.0,
                    ValueKind.Minimum => dbType == DbType.Double ? double.MinValue : float.MinValue,
                    ValueKind.Maximum => dbType == DbType.Double ? double.MaxValue : float.MaxValue,
                    ValueKind.Empty => 0.0, // Treat Empty as Zero
                    _ => throw new ArgumentOutOfRangeException(nameof(kind)),
                };
                // Handle NaN, Infinity if needed (BigQuery supports these)
                if (double.IsNaN(doubleValue)) return "CAST('NaN' AS FLOAT64)";
                if (double.IsPositiveInfinity(doubleValue)) return "CAST('+inf' AS FLOAT64)";
                if (double.IsNegativeInfinity(doubleValue)) return "CAST('-inf' AS FLOAT64)";
                // Use "G17" for round-trip fidelity for double
                return doubleValue.ToString("G17", CultureInfo.InvariantCulture);

            // Boolean
            case DbType.Boolean:
                bool boolValue = kind switch
                {
                    ValueKind.Zero => false,
                    ValueKind.One => true,
                    ValueKind.Minimum => false, // Define Min/Max for bool
                    ValueKind.Maximum => true,
                    ValueKind.Empty => false, // Treat Empty as false
                    _ => throw new ArgumentOutOfRangeException(nameof(kind)),
                };
                return boolValue.ToString().ToLowerInvariant(); // 'true' or 'false'

            // Strings & Guid
            case DbType.String:
            case DbType.AnsiString:
            case DbType.StringFixedLength:
            case DbType.AnsiStringFixedLength:
            case DbType.Guid:
                string stringValue = kind switch
                {
                    ValueKind.Zero => "0", // Represent zero as string "0"
                    ValueKind.One => "1",   // Represent one as string "1"
                    ValueKind.Minimum => "A", // Define Min/Max for strings if needed
                    ValueKind.Maximum => "ZZZZZ",
                    ValueKind.Empty => "", // Empty string
                    _ => throw new ArgumentOutOfRangeException(nameof(kind)),
                };
                // Escape single quotes and backslashes for BigQuery string literal
                return $"'{stringValue.Replace("\\", "\\\\").Replace("'", "\\'")}'";

            // Binary -> Delegate to CreateSelectSql(byte[]) which uses Base64
            case DbType.Binary:
                byte[] bytesValue = kind switch
                {
                    ValueKind.Zero => new byte[] { 0 },
                    ValueKind.One => new byte[] { 1 },
                    ValueKind.Minimum => new byte[] { 0x00 },
                    ValueKind.Maximum => new byte[] { 0xFF },
                    ValueKind.Empty => Array.Empty<byte>(),
                    _ => throw new ArgumentOutOfRangeException(nameof(kind)),
                };
                return CreateSelectSql(bytesValue).Substring("SELECT ".Length).Replace(" AS Value", ""); // Extract literal part

            // Date/Time types
            case DbType.Date:
                DateTime dateValue = kind switch
                {
                    ValueKind.Zero => new DateTime(1, 1, 1), // BQ min date
                    ValueKind.One => new DateTime(1, 1, 2), // Day after min
                    ValueKind.Minimum => new DateTime(1, 1, 1), // BQ min date
                    ValueKind.Maximum => new DateTime(9999, 12, 31), // BQ max date
                    ValueKind.Empty => new DateTime(1, 1, 1), // Treat Empty as Min
                    _ => throw new ArgumentOutOfRangeException(nameof(kind)),
                };
                return $"DATE '{dateValue:yyyy-MM-dd}'";

            case DbType.Time:
                TimeSpan timeValue = kind switch
                {
                    ValueKind.Zero => TimeSpan.Zero, // 00:00:00
                    ValueKind.One => TimeSpan.FromTicks(10000), // Smallest representable tick as 1 microsecond? Or 1 second? Let's use 1 microsecond.
                    ValueKind.Minimum => TimeSpan.Zero,
                    ValueKind.Maximum => TimeSpan.Parse("23:59:59.999999"), // Max BQ time
                    ValueKind.Empty => TimeSpan.Zero, // Treat Empty as Zero
                    _ => throw new ArgumentOutOfRangeException(nameof(kind)),
                };
                return $"TIME '{timeValue:hh\\:mm\\:ss\\.ffffff}'";

            case DbType.DateTime:
            case DbType.DateTime2:
                DateTime dtValue = kind switch
                {
                    ValueKind.Zero => new DateTime(1, 1, 1, 0, 0, 0), // BQ min datetime
                    ValueKind.One => new DateTime(1, 1, 1, 0, 0, 0, 1), // Smallest fraction
                    ValueKind.Minimum => new DateTime(1, 1, 1, 0, 0, 0),
                    ValueKind.Maximum => new DateTime(9999, 12, 31, 23, 59, 59, 999).AddTicks(9990), // BQ max precision
                    ValueKind.Empty => new DateTime(1, 1, 1, 0, 0, 0), // Treat Empty as Min
                    _ => throw new ArgumentOutOfRangeException(nameof(kind)),
                };
                return $"DATETIME '{dtValue:yyyy-MM-dd HH:mm:ss.ffffff}'";

            case DbType.DateTimeOffset: // TIMESTAMP
                DateTimeOffset dtoValue = kind switch
                {
                    ValueKind.Zero => new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero), // Unix Epoch often represents Zero
                    ValueKind.One => new DateTimeOffset(1970, 1, 1, 0, 0, 1, TimeSpan.Zero), // One second after epoch
                    ValueKind.Minimum => new DateTimeOffset(1, 1, 1, 0, 0, 0, TimeSpan.Zero), // BQ min timestamp
                    ValueKind.Maximum => new DateTimeOffset(9999, 12, 31, 23, 59, 59, 999, TimeSpan.Zero).AddTicks(9990), // BQ max timestamp
                    ValueKind.Empty => new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero), // Treat Empty as Epoch/Zero
                    _ => throw new ArgumentOutOfRangeException(nameof(kind)),
                };
                return $"TIMESTAMP '{dtoValue:yyyy-MM-dd HH:mm:ss.ffffff zzz}'";


            // Numeric types
            case DbType.Decimal: // NUMERIC
                decimal decValue = kind switch
                {
                    ValueKind.Zero => 0m,
                    ValueKind.One => 1m,
                    // Using smaller range within C# decimal limits but valid BQ NUMERIC
                    ValueKind.Minimum => -99999999999999999999.999999999m,
                    ValueKind.Maximum => 99999999999999999999.999999999m,
                    ValueKind.Empty => 0m, // Treat Empty as Zero
                    _ => throw new ArgumentOutOfRangeException(nameof(kind)),
                };
                return $"NUMERIC '{decValue.ToString(CultureInfo.InvariantCulture)}'";

            case DbType.VarNumeric:
                string bignumString = kind switch
                {
                    ValueKind.Zero => "0",
                    ValueKind.One => "1",
                    ValueKind.Minimum => "-578960446186580977117854925043439539266.34992332820282019728792003956564819968",
                    ValueKind.Maximum => "578960446186580977117854925043439539266.34992332820282019728792003956564819967",
                    ValueKind.Empty => "0", // Treat Empty as Zero
                    _ => throw new ArgumentOutOfRangeException(nameof(kind)),
                };
                return $"BIGNUMERIC '{bignumString}'";

            default:
                throw new NotSupportedException($"DbValue literal generation not implemented for DbType '{dbType}'.");
        }
    }


}