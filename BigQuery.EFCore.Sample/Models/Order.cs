using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ivy.EFCore.BigQuery.Sample.Models;

[Table("orders")]
public partial class Order
{
    [Key]
    [Column("order_id", TypeName = "integer")]
    public int? OrderId { get; set; }

    [Column("user_id", TypeName = "integer")]
    public int? UserId { get; set; }

    [Column("status", TypeName = "string")]
    public string? Status { get; set; }

    [Column("gender", TypeName = "string")]
    public string? Gender { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime? CreatedAt { get; set; }

    [Column("returned_at", TypeName = "timestamp")]
    public DateTime? ReturnedAt { get; set; }

    [Column("shipped_at", TypeName = "timestamp")]
    public DateTime? ShippedAt { get; set; }

    [Column("delivered_at", TypeName = "timestamp")]
    public DateTime? DeliveredAt { get; set; }

    [Column("num_of_item", TypeName = "integer")]
    public int? NumOfItem { get; set; }
}
