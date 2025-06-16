using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ivy.EFCore.BigQuery.Sample.Models;

[Table("distribution_centers")]
public partial class DistributionCenter
{
    [Key]
    [Column("id", TypeName = "integer")]
    public int? Id { get; set; }

    [Column("name", TypeName = "string")]
    public string? Name { get; set; }

    [Column("latitude", TypeName = "float")]
    public float? Latitude { get; set; }

    [Column("longitude", TypeName = "float")]
    public float? Longitude { get; set; }

    [Column("distribution_center_geom", TypeName = "string")]
    public string? DistributionCenterGeom { get; set; }
}
