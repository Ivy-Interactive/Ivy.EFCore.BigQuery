
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Ivy.EFCore.BigQuery.FunctionalTests.TestUtilities
{
    public class BigQueryNorthwindModelCustomizer : ITestModelCustomizer
    {
        public void Customize(ModelBuilder modelBuilder, DbContext context)
        {
            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderDetail>().ToTable("Order Details");
            modelBuilder.Entity<Product>().ToTable("Products");
            //modelBuilder.Entity<Category>().ToTable("Categories");
            //modelBuilder.Entity<Shipper>().ToTable("Shippers");
        }

        public void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
        }
    }
}
