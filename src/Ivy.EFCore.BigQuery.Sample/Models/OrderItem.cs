using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ivy.EFCore.BigQuery.Sample.Models;

[Table("order_items")]
public partial class OrderItem
{
    [Key]
    [Column("id", TypeName = "integer")]
    public int? Id { get; set; }

    [Column("order_id", TypeName = "integer")]
    public int? OrderId { get; set; }

    [Column("user_id", TypeName = "integer")]
    public int? UserId { get; set; }

    [Column("product_id", TypeName = "integer")]
    public int? ProductId { get; set; }

    [Column("inventory_item_id", TypeName = "integer")]
    public int? InventoryItemId { get; set; }

    [Column("status", TypeName = "string")]
    public string? Status { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime? CreatedAt { get; set; }

    [Column("shipped_at", TypeName = "timestamp")]
    public DateTime? ShippedAt { get; set; }

    [Column("delivered_at", TypeName = "timestamp")]
    public DateTime? DeliveredAt { get; set; }

    [Column("returned_at", TypeName = "timestamp")]
    public DateTime? ReturnedAt { get; set; }

    [Column("sale_price", TypeName = "float")]
    public float? SalePrice { get; set; }
}
