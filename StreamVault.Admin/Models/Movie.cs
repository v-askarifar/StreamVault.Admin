using System.ComponentModel.DataAnnotations;

namespace StreamVault.Admin.Models;

public class Movie : CatalogueItem
{
    [Range(1, 1000)]
    public int DurationMinutes { get; set; }

    [Required]
    [StringLength(120)]
    public string Director { get; set; } = string.Empty;

    public override string ContentTypeKey => ContentTypes.Movie;

    public override string TypeBadgeClass => "text-bg-primary";

    public override string SpecificSummary => $"{DurationMinutes} min · Directed by {Director}";

    public override int DashboardDurationMinutes => DurationMinutes;
}