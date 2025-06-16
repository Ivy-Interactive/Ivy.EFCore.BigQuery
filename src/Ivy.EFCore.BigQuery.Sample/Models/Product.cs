using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ivy.EFCore.BigQuery.Sample.Models;

[Table("products")]
public partial class Product
{
    [Key]
    [Column("id", TypeName = "integer")]
    public int? Id { get; set; }

    [Column("cost", TypeName = "float")]
    public float? Cost { get; set; }

    [Column("category", TypeName = "string")]
    public string? Category { get; set; }

    [Column("name", TypeName = "string")]
    public string? Name { get; set; }

    [Column("brand", TypeName = "string")]
    public string? Brand { get; set; }

    [Column("retail_price", TypeName = "float")]
    public float? RetailPrice { get; set; }

    [Column("department", TypeName = "string")]
    public string? Department { get; set; }

    [Column("sku", TypeName = "string")]
    public string? Sku { get; set; }

    [Column("distribution_center_id", TypeName = "integer")]
    public int? DistributionCenterId { get; set; }
}
