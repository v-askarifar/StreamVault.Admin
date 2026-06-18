using System.ComponentModel.DataAnnotations;
using StreamVault.Admin.Models;
using StreamVault.Admin.ViewModels;

namespace StreamVault.Admin.Tests;

public class CatalogueFormViewModelTests
{
    [Fact]
    public void Movie_WithoutDirector_IsInvalid()
    {
        var model = ValidBaseModel(ContentTypes.Movie);
        model.DurationMinutes = 120;
        model.Director = " ";

        var results = Validate(model);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(model.Director)));
    }

    [Fact]
    public void Movie_WithPositiveDurationAndDirector_IsValid()
    {
        var model = ValidBaseModel(ContentTypes.Movie);
        model.DurationMinutes = 120;
        model.Director = "Christopher Nolan";

        var results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void Series_WithFewerEpisodesThanSeasons_IsInvalid()
    {
        var model = ValidBaseModel(ContentTypes.Series);
        model.NumberOfSeasons = 5;
        model.TotalEpisodes = 3;

        var results = Validate(model);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(model.TotalEpisodes)));
    }

    [Fact]
    public void MusicAlbum_WithoutTrackCount_IsInvalid()
    {
        var model = ValidBaseModel(ContentTypes.MusicAlbum);
        model.Artist = "Daft Punk";

        var results = Validate(model);

        Assert.Contains(results, x => x.MemberNames.Contains(nameof(model.TrackCount)));
    }

    private static CatalogueFormViewModel ValidBaseModel(string contentType)
    {
        return new CatalogueFormViewModel
        {
            ContentType = contentType,
            Title = "Test title",
            ReleaseDate = new DateTime(2024, 1, 1),
            AgeRating = "PG",
            Genre = "Drama"
        };
    }

    private static List<ValidationResult> Validate(CatalogueFormViewModel model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }
}
