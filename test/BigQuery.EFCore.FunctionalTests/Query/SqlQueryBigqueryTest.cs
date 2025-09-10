using Ivy.Data.BigQuery;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Ivy.EFCore.BigQuery.FunctionalTests.Query;

public class SqlQueryBigQueryTest : SqlQueryTestBase<NorthwindQueryBigQueryFixture<NoopModelCustomizer>>
{
    public SqlQueryBigQueryTest(NorthwindQueryBigQueryFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override async Task SqlQueryRaw_queryable_simple(bool async)
    {
        await base.SqlQueryRaw_queryable_simple(async);

        AssertSql(
            """
SELECT
    *
  FROM
    Customers
  WHERE Customers.ContactName LIKE '%z%'
;
""");
    }

    protected override DbParameter CreateDbParameter(string name, object value)
    => new BigQueryParameter { ParameterName = name, Value = value };

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}