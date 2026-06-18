using StreamVault.Admin.Models;
using StreamVault.Admin.Services;
using StreamVault.Admin.ViewModels;

namespace StreamVault.Admin.Tests;

public class CatalogueItemMapperTests
{
    private readonly CatalogueItemMapper _mapper = new();

    [Fact]
    public void ToEntity_CreatesMovieWithTrimmedValues()
    {
        var model = new CatalogueFormViewModel
        {
            ContentType = ContentTypes.Movie,
            Title = "  Inception  ",
            Description = "  A sci-fi film  ",
            ReleaseDate = new DateTime(2010, 7, 16),
            AgeRating = "  12A  ",
            Genre = "  Sci-Fi  ",
            DurationMinutes = 148,
            Director = "  Christopher Nolan  "
        };

        var item = Assert.IsType<Movie>(_mapper.ToEntity(model));

        Assert.Equal("Inception", item.Title);
        Assert.Equal("A sci-fi film", item.Description);
        Assert.Equal("12A", item.AgeRating);
        Assert.Equal("Sci-Fi", item.Genre);
        Assert.Equal(148, item.DurationMinutes);
        Assert.Equal("Christopher Nolan", item.Director);
    }

    [Fact]
    public void ToFormModel_MapsMusicAlbumFields()
    {
        var album = new MusicAlbum
        {
            Id = 10,
            PublicId = Guid.NewGuid(),
            Title = "Discovery",
            ReleaseDate = new DateTime(2001, 3, 12),
            AgeRating = "PG",
            Genre = "Electronic",
            Artist = "Daft Punk",
            TrackCount = 14,
            RecordLabel = "Virgin"
        };

        var model = _mapper.ToFormModel(album);

        Assert.Equal(ContentTypes.MusicAlbum, model.ContentType);
        Assert.Equal("Daft Punk", model.Artist);
        Assert.Equal(14, model.TrackCount);
        Assert.Equal("Virgin", model.RecordLabel);
    }
}
