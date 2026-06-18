using Microsoft.EntityFrameworkCore;
using StreamVault.Admin.Data;
using StreamVault.Admin.Models;
using StreamVault.Admin.ViewModels;

namespace StreamVault.Admin.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var items = await _context.CatalogueItems
            .AsNoTracking()
            .ToListAsync();

        return new DashboardViewModel
        {
            TotalItems = items.Count,

            MovieCount = items.Count(x => x.ContentTypeKey == ContentTypes.Movie),
            SeriesCount = items.Count(x => x.ContentTypeKey == ContentTypes.Series),
            AudiobookCount = items.Count(x => x.ContentTypeKey == ContentTypes.Audiobook),
            MusicAlbumCount = items.Count(x => x.ContentTypeKey == ContentTypes.MusicAlbum),

            TotalDurationMinutes = items.Sum(x => x.DashboardDurationMinutes),
            TotalEpisodes = items.Sum(x => x.DashboardEpisodeCount),
            TotalTracks = items.Sum(x => x.DashboardTrackCount),

            RecentItems = items
                .OrderByDescending(x => x.ReleaseDate)
                .Take(5)
                .Select(ToRecentItem)
                .ToList()
        };
    }

    private static RecentCatalogueItemViewModel ToRecentItem(CatalogueItem item)
    {
        return new RecentCatalogueItemViewModel
        {
            PublicId = item.PublicId,
            Title = item.Title,
            ContentType = item.ContentTypeName,
            ReleaseDate = item.ReleaseDate,
            Genre = item.Genre
        };
    }
}