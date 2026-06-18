using System.ComponentModel.DataAnnotations;

namespace StreamVault.Admin.Models;

public class Series : CatalogueItem
{
    [Range(1, 100)]
    public int NumberOfSeasons { get; set; }

    [Range(1, 10000)]
    public int TotalEpisodes { get; set; }

    public override string ContentTypeKey => ContentTypes.Series;

    public override string TypeBadgeClass => "text-bg-success";

    public override string SpecificSummary => $"{NumberOfSeasons} seasons · {TotalEpisodes} episodes";

    public override int DashboardEpisodeCount => TotalEpisodes;
}