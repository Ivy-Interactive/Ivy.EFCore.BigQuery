using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ivy.EFCore.BigQuery.Sample.Models;

[Table("users")]
public partial class User
{
    [Key]
    [Column("id", TypeName = "integer")]
    public int? Id { get; set; }

    [Column("first_name", TypeName = "string")]
    public string? FirstName { get; set; }

    [Column("last_name", TypeName = "string")]
    public string? LastName { get; set; }

    [Column("email", TypeName = "string")]
    public string? Email { get; set; }

    [Column("age", TypeName = "integer")]
    public int? Age { get; set; }

    [Column("gender", TypeName = "string")]
    public string? Gender { get; set; }

    [Column("state", TypeName = "string")]
    public string? State { get; set; }

    [Column("street_address", TypeName = "string")]
    public string? StreetAddress { get; set; }

    [Column("postal_code", TypeName = "string")]
    public string? PostalCode { get; set; }

    [Column("city", TypeName = "string")]
    public string? City { get; set; }

    [Column("country", TypeName = "string")]
    public string? Country { get; set; }

    [Column("latitude", TypeName = "float")]
    public float? Latitude { get; set; }

    [Column("longitude", TypeName = "float")]
    public float? Longitude { get; set; }

    [Column("traffic_source", TypeName = "string")]
    public string? TrafficSource { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime? CreatedAt { get; set; }

    [Column("user_geom", TypeName = "string")]
    public string? UserGeom { get; set; }
}
