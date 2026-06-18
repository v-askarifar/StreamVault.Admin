using System.ComponentModel.DataAnnotations;

namespace StreamVault.Admin.Models;

public class MusicAlbum : CatalogueItem
{
    [Required]
    [StringLength(120)]
    public string Artist { get; set; } = string.Empty;

    [Range(1, 500)]
    public int TrackCount { get; set; }

    [StringLength(120)]
    public string? RecordLabel { get; set; }

    public override string ContentTypeKey => ContentTypes.MusicAlbum;

    public override string TypeBadgeClass => "text-bg-danger";

    public override string SpecificSummary => $"{TrackCount} tracks · {Artist}";

    public override int DashboardTrackCount => TrackCount;
}