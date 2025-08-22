using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Ivy.EFCore.BigQuery.Design.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using Ivy.EFCore.BigQuery.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Ivy.Data.BigQuery;

var dataSource = "http://localhost:9050";
var projectId = "test";
var datasetId = "ado_tests";

var connString = $"DataSource={dataSource};AuthMethod=ApplicationDefaultCredentials;DefaultDataset={datasetId};ProjectId={projectId}";

//"path/to/credentials.json"
//);

//DemoEf();



//using (var context = new BigQueryContext(projectId, datasetId))
//{
//    var items = context.Query<ItemEntity>("inventory_items");

//    foreach (var item in items)
//    {
//        Console.WriteLine($"Id: {item.Id}, Name: {item.ProductName}, CreatedAt: {item.CreatedAt}");
//    }

//var centers = context.Query<DistributionCenter>("distribution_centers");

//foreach (var item in centers)
//{
//    Console.WriteLine($"Id: {item.Id}, Name: {item.Name}, Geometry: {item.DistributionCenterGeom}");
//}

//Console.ReadKey();
//}


//var connString = "AuthMethod=ApplicationDefaultCredentials;ProjectId=sublime-blade-329603";


await DemoAdo(connString);


async Task DemoAdo(string connectionString)
{
    try
    {
        using (var connection = new BigQueryConnection(connectionString))
        {
            await connection.OpenAsync();
            Console.WriteLine($"Connection State: {connection.State}");
            Console.WriteLine($"Project ID (Database): {connection.Database}");
            Console.WriteLine($"Default Dataset: {connection.DefaultDatasetId}");

            // Create a command
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM `sublime-blade-329603.reltest.user_activity_log` LIMIT 1000";

                //var param = new BigQueryParameter("@StateParam", Google.Cloud.BigQuery.V2.BigQueryDbType.String)
                //{
                //    Value = "CA"
                //};
                //command.Parameters.Add(param);

                Console.WriteLine("\nExecuting query");
             
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    Console.WriteLine("Results:");
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"Id: {reader["log_id"]} User: {reader["user_id"]}");
                    }
                }
            }

            Console.WriteLine($"\nConnection State after using: {connection.State}");
        }
    }
    catch (BigQueryException bqEx)
    {
        Console.Error.WriteLine($"BigQuery Error: {bqEx.Message}");
        if (bqEx.ErrorProto != null)
        {
            Console.Error.WriteLine($"  Reason: {bqEx.ErrorProto.Reason}, Location: {bqEx.ErrorProto.Location}");
        }
        // Handle specific BQ errors
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"General Error: {ex.ToString()}");
        // Handle other errors (auth, network, etc.)
    }
}

void Scaffold()
{
    var services = new ServiceCollection();
    var designTimeServices = new BigQueryDesignTimeServices();
    designTimeServices.ConfigureDesignTimeServices(services);

    var serviceProvider = services.BuildServiceProvider();
    var databaseFactory = serviceProvider.GetService<IDatabaseModelFactory>();

    if (databaseFactory != null)
    {
        var dbModel = databaseFactory.Create("your_bigquery_connection", new DatabaseModelFactoryOptions());
        Console.WriteLine($"Scaffolded {dbModel.Tables.Count} tables.");
    }
    else
    {
        Console.WriteLine("Failed to resolve DatabaseModelFactory.");
    }
}

void DemoEf()
{
    var serviceCollection = new ServiceCollection();
    serviceCollection.AddBigQuery<TestDbContext>(
        "project",
        "dataset",
        bigqueryOptions =>
        {
            bigqueryOptions.MaxBatchSize(123);
            bigqueryOptions.CommandTimeout(30);
        },
        dbContextOption =>
        {
            _ = 1;
        });
    var services = serviceCollection.BuildServiceProvider(validateScopes: true);

    var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetService<TestDbContext>();
    var items = db.Items.Take(100);
    var itemsList = items.ToList();
}

public class ItemEntity
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("product_id")]
    public int? ProductId { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("sold_at")]
    public DateTime? SoldAt { get; set; }

    [Column("cost")]
    public float? Cost { get; set; }

    [Column("product_category")]
    public string ProductCategory { get; set; }

    [Column("product_name")]
    public string ProductName { get; set; }

    [Column("product_brand")]
    public string ProductBrand { get; set; }

    [Column("product_retail_price")]
    public float? ProductRetailPrice { get; set; }

    [Column("product_department")]
    public string ProductDepartment { get; set; }

    [Column("product_sku")]
    public string ProductSku { get; set; }

    [Column("product_distribution_center_id")]
    public int? ProductDistributionCenterId { get; set; }
}

//public class BigQueryContext : DbContext
//{
//    private readonly string _projectId;
//    private readonly string _datasetId;
//    private readonly string _credentialsPath;

//    public BigQueryContext(string projectId, string datasetId)
//        //, string credentialsPath)
//    {
//        _projectId = projectId;
//        _datasetId = datasetId;
//        //_credentialsPath = credentialsPath;
//    }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//    {
//        optionsBuilder.UseBigQuery(_projectId, _datasetId);
//            //_credentialsPath);
//    }
//}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ItemEntity> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}