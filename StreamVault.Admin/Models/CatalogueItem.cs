using System.ComponentModel.DataAnnotations;

namespace StreamVault.Admin.Models;

public abstract class CatalogueItem
{
    public int Id { get; set; }

    public Guid PublicId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime ReleaseDate { get; set; }

    [Required]
    [StringLength(20)]
    public string AgeRating { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Genre { get; set; } = string.Empty;

    public abstract string ContentTypeKey { get; }

    public string ContentTypeName => ContentTypes.GetDisplayName(ContentTypeKey);

    public abstract string TypeBadgeClass { get; }

    public abstract string SpecificSummary { get; }

    public virtual int DashboardDurationMinutes => 0;

    public virtual int DashboardEpisodeCount => 0;

    public virtual int DashboardTrackCount => 0;
}