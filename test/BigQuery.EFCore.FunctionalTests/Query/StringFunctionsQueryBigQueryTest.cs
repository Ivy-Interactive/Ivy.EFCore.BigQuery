
using Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using System.Linq;
using Xunit;

namespace Ivy.EFCore.BigQuery.FunctionalTests.Query
{
    public class StringFunctionsQueryBigQueryTest : IClassFixture<NorthwindQueryBigQueryFixture<BigQueryNorthwindModelCustomizer>>
    {
        private readonly NorthwindQueryBigQueryFixture<BigQueryNorthwindModelCustomizer> _fixture;

        public StringFunctionsQueryBigQueryTest(NorthwindQueryBigQueryFixture<BigQueryNorthwindModelCustomizer> fixture)
        {
            _fixture = fixture;
            _fixture.TestSqlLoggerFactory.Clear();
        }

        [Fact]
        public void String_ToLower_is_translated()
        {
            using var context = _fixture.CreateContext();
            var _ = context.Customers.Where(c => c.CustomerID.ToLower() == "alfki").ToList();

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LOWER(`c`.`CustomerID`) = 'alfki'
""");
        }

        [Fact]
        public void String_ToUpper_is_translated()
        {
            using var context = _fixture.CreateContext();
            var _ = context.Customers.Where(c => c.CustomerID.ToUpper() == "ALFKI").ToList();

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE UPPER(`c`.`CustomerID`) = 'ALFKI'
""");
        }

        [Fact]
        public void String_StartsWith_is_translated()
        {
            using var context = _fixture.CreateContext();
            var _ = context.Customers.Where(c => c.ContactName.StartsWith("Maria")).ToList();

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE 'Maria%'
""");
        }

        private void AssertSql(string expected)
        {
            _fixture.TestSqlLoggerFactory.AssertBaseline(new[] { expected });
        }
    }
}
