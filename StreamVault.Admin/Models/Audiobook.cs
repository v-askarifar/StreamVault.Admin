using System.ComponentModel.DataAnnotations;

namespace StreamVault.Admin.Models;

public class Audiobook : CatalogueItem
{
    [Required]
    [StringLength(120)]
    public string Author { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Narrator { get; set; } = string.Empty;

    [Range(1, 10000)]
    public int DurationMinutes { get; set; }

    public override string ContentTypeKey => ContentTypes.Audiobook;

    public override string TypeBadgeClass => "text-bg-warning";

    public override string SpecificSummary => $"{DurationMinutes} min · {Author} · Narrated by {Narrator}";

    public override int DashboardDurationMinutes => DurationMinutes;
}