namespace StreamVault.Admin.ViewModels;

public class DashboardViewModel
{
    public int TotalItems { get; set; }

    public int MovieCount { get; set; }

    public int SeriesCount { get; set; }

    public int AudiobookCount { get; set; }

    public int MusicAlbumCount { get; set; }

    public int TotalDurationMinutes { get; set; }

    public int TotalEpisodes { get; set; }

    public int TotalTracks { get; set; }

    public IReadOnlyList<RecentCatalogueItemViewModel> RecentItems { get; set; } = [];
}

public class RecentCatalogueItemViewModel
{
    public Guid PublicId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public DateTime ReleaseDate { get; set; }

    public string Genre { get; set; } = string.Empty;
}