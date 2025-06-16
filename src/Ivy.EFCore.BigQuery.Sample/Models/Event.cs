using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ivy.EFCore.BigQuery.Sample.Models;

[Table("events")]
public partial class Event
{
    [Key]
    [Column("id", TypeName = "integer")]
    public int? Id { get; set; }

    [Column("user_id", TypeName = "integer")]
    public int? UserId { get; set; }

    [Column("sequence_number", TypeName = "integer")]
    public int? SequenceNumber { get; set; }

    [Column("session_id", TypeName = "string")]
    public string? SessionId { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime? CreatedAt { get; set; }

    [Column("ip_address", TypeName = "string")]
    public string? IpAddress { get; set; }

    [Column("city", TypeName = "string")]
    public string? City { get; set; }

    [Column("state", TypeName = "string")]
    public string? State { get; set; }

    [Column("postal_code", TypeName = "string")]
    public string? PostalCode { get; set; }

    [Column("browser", TypeName = "string")]
    public string? Browser { get; set; }

    [Column("traffic_source", TypeName = "string")]
    public string? TrafficSource { get; set; }

    [Column("uri", TypeName = "string")]
    public string? Uri { get; set; }

    [Column("event_type", TypeName = "string")]
    public string? EventType { get; set; }
}
