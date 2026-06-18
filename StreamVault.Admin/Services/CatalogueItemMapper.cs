using StreamVault.Admin.Models;
using StreamVault.Admin.ViewModels;

namespace StreamVault.Admin.Services;

public class CatalogueItemMapper
{
    public CatalogueItem ToEntity(CatalogueFormViewModel model)
    {
        var item = CreateTypedEntity(model.ContentType);
        UpdateCommonFields(model, item);
        UpdateTypeSpecificFields(model, item);
        return item;
    }

    public CatalogueFormViewModel ToFormModel(CatalogueItem item)
    {
        var model = new CatalogueFormViewModel
        {
            Id = item.Id,
            PublicId = item.PublicId,
            ContentType = item.ContentTypeKey,
            Title = item.Title,
            Description = item.Description,
            ReleaseDate = item.ReleaseDate,
            AgeRating = item.AgeRating,
            Genre = item.Genre
        };

        CopyTypeSpecificFields(item, model);
        return model;
    }

    public void UpdateEntity(CatalogueFormViewModel model, CatalogueItem item)
    {
        UpdateCommonFields(model, item);
        UpdateTypeSpecificFields(model, item);
    }

    private static CatalogueItem CreateTypedEntity(string contentType)
    {
        return contentType switch
        {
            ContentTypes.Movie => new Movie(),
            ContentTypes.Series => new Series(),
            ContentTypes.Audiobook => new Audiobook(),
            ContentTypes.MusicAlbum => new MusicAlbum(),
            _ => throw new InvalidOperationException("Invalid content type.")
        };
    }

    private static void UpdateCommonFields(CatalogueFormViewModel model, CatalogueItem item)
    {
        item.Title = CleanRequired(model.Title);
        item.Description = CleanOptional(model.Description);
        item.ReleaseDate = model.ReleaseDate;
        item.AgeRating = CleanRequired(model.AgeRating);
        item.Genre = CleanRequired(model.Genre);
    }

    private static void CopyTypeSpecificFields(CatalogueItem item, CatalogueFormViewModel model)
    {
        switch (item)
        {
            case Movie movie:
                model.DurationMinutes = movie.DurationMinutes;
                model.Director = movie.Director;
                break;

            case Series series:
                model.NumberOfSeasons = series.NumberOfSeasons;
                model.TotalEpisodes = series.TotalEpisodes;
                break;

            case Audiobook audiobook:
                model.Author = audiobook.Author;
                model.Narrator = audiobook.Narrator;
                model.DurationMinutes = audiobook.DurationMinutes;
                break;

            case MusicAlbum album:
                model.Artist = album.Artist;
                model.TrackCount = album.TrackCount;
                model.RecordLabel = album.RecordLabel;
                break;

            default:
                throw new InvalidOperationException("Unsupported content item type.");
        }
    }

    private static void UpdateTypeSpecificFields(CatalogueFormViewModel model, CatalogueItem item)
    {
        switch (item)
        {
            case Movie movie:
                movie.DurationMinutes = model.DurationMinutes!.Value;
                movie.Director = CleanRequired(model.Director);
                break;

            case Series series:
                series.NumberOfSeasons = model.NumberOfSeasons!.Value;
                series.TotalEpisodes = model.TotalEpisodes!.Value;
                break;

            case Audiobook audiobook:
                audiobook.Author = CleanRequired(model.Author);
                audiobook.Narrator = CleanRequired(model.Narrator);
                audiobook.DurationMinutes = model.DurationMinutes!.Value;
                break;

            case MusicAlbum album:
                album.Artist = CleanRequired(model.Artist);
                album.TrackCount = model.TrackCount!.Value;
                album.RecordLabel = CleanOptional(model.RecordLabel);
                break;

            default:
                throw new InvalidOperationException("Unsupported content item type.");
        }
    }

    private static string CleanRequired(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    private static string? CleanOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
