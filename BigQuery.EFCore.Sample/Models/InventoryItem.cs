using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ivy.EFCore.BigQuery.Sample.Models;

[Table("inventory_items")]
public partial class InventoryItem
{
    [Key]
    [Column("id", TypeName = "integer")]
    public int? Id { get; set; }

    [Column("product_id", TypeName = "integer")]
    public int? ProductId { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime? CreatedAt { get; set; }

    [Column("sold_at", TypeName = "timestamp")]
    public DateTime? SoldAt { get; set; }

    [Column("cost", TypeName = "float")]
    public float? Cost { get; set; }

    [Column("product_category", TypeName = "string")]
    public string? ProductCategory { get; set; }

    [Column("product_name", TypeName = "string")]
    public string? ProductName { get; set; }

    [Column("product_brand", TypeName = "string")]
    public string? ProductBrand { get; set; }

    [Column("product_retail_price", TypeName = "float")]
    public float? ProductRetailPrice { get; set; }

    [Column("product_department", TypeName = "string")]
    public string? ProductDepartment { get; set; }

    [Column("product_sku", TypeName = "string")]
    public string? ProductSku { get; set; }

    [Column("product_distribution_center_id", TypeName = "integer")]
    public int? ProductDistributionCenterId { get; set; }
}
