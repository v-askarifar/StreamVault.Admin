using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StreamVault.Admin.Data;
using StreamVault.Admin.Models;
using StreamVault.Admin.Services;
using StreamVault.Admin.ViewModels;

namespace StreamVault.Admin.Tests;

public class CatalogueServiceTests
{
    [Fact]
    public async Task CreateAsync_AddsMovieToDatabase()
    {
        await using var fixture = new TestDatabaseFixture();
        var service = CreateService(fixture.Context);

        var model = ValidMovieModel("Inception");

        await service.CreateAsync(model);

        var movie = await fixture.Context.CatalogueItems.OfType<Movie>().SingleAsync();

        Assert.Equal("Inception", movie.Title);
        Assert.Equal(148, movie.DurationMinutes);
        Assert.Equal("Christopher Nolan", movie.Director);
    }

    [Fact]
    public async Task GetListAsync_ReturnsItemsOrderedByTitle()
    {
        await using var fixture = new TestDatabaseFixture();
        fixture.Context.CatalogueItems.AddRange(
            ValidMovie("Zodiac"),
            ValidMovie("Arrival"),
            ValidSeries("Breaking Bad")
        );
        await fixture.Context.SaveChangesAsync();

        var service = CreateService(fixture.Context);

        var result = await service.GetListAsync(type: null, search: null);

        Assert.Equal(["Arrival", "Breaking Bad", "Zodiac"], result.Items.Select(x => x.Title));
    }

    [Fact]
    public async Task GetListAsync_FiltersByContentType()
    {
        await using var fixture = new TestDatabaseFixture();
        fixture.Context.CatalogueItems.AddRange(
            ValidMovie("Inception"),
            ValidSeries("Planet Earth"),
            ValidAudiobook("Atomic Habits"),
            ValidMusicAlbum("Discovery")
        );
        await fixture.Context.SaveChangesAsync();

        var service = CreateService(fixture.Context);

        var result = await service.GetListAsync(ContentTypes.Movie, search: null);

        var item = Assert.Single(result.Items);
        Assert.IsType<Movie>(item);
        Assert.Equal(ContentTypes.Movie, result.SelectedType);
    }

    [Fact]
    public async Task GetListAsync_SearchesByTrimmedTitle()
    {
        await using var fixture = new TestDatabaseFixture();
        fixture.Context.CatalogueItems.AddRange(
            ValidMovie("Inception"),
            ValidMovie("Interstellar"),
            ValidSeries("Planet Earth")
        );
        await fixture.Context.SaveChangesAsync();

        var service = CreateService(fixture.Context);

        var result = await service.GetListAsync(type: null, search: "  Inter  ");

        var item = Assert.Single(result.Items);
        Assert.Equal("Interstellar", item.Title);
    }

    [Fact]
    public async Task GetForEditAsync_ReturnsMappedFormModel()
    {
        await using var fixture = new TestDatabaseFixture();

        var movie = ValidMovie("Inception");
        fixture.Context.CatalogueItems.Add(movie);
        await fixture.Context.SaveChangesAsync();

        var service = CreateService(fixture.Context);

        var result = await service.GetForEditAsync(movie.PublicId);

        Assert.NotNull(result);
        Assert.Equal(movie.PublicId, result.PublicId);
        Assert.Equal(ContentTypes.Movie, result.ContentType);
        Assert.Equal("Inception", result.Title);
        Assert.Equal(148, result.DurationMinutes);
        Assert.Equal("Christopher Nolan", result.Director);
    }

    [Fact]
    public async Task GetForEditAsync_ReturnsNull_WhenItemDoesNotExist()
    {
        await using var fixture = new TestDatabaseFixture();
        var service = CreateService(fixture.Context);

        var result = await service.GetForEditAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingItem()
    {
        await using var fixture = new TestDatabaseFixture();

        var movie = ValidMovie("Old title");
        fixture.Context.CatalogueItems.Add(movie);
        await fixture.Context.SaveChangesAsync();

        var service = CreateService(fixture.Context);

        var updateModel = ValidMovieModel("Updated title");
        updateModel.PublicId = movie.PublicId;
        updateModel.DurationMinutes = 150;
        updateModel.Director = "Denis Villeneuve";

        var result = await service.UpdateAsync(movie.PublicId, updateModel);

        var updatedMovie = await fixture.Context.CatalogueItems.OfType<Movie>().SingleAsync();

        Assert.Equal(UpdateCatalogueResult.Success, result);
        Assert.Equal("Updated title", updatedMovie.Title);
        Assert.Equal(150, updatedMovie.DurationMinutes);
        Assert.Equal("Denis Villeneuve", updatedMovie.Director);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenItemDoesNotExist()
    {
        await using var fixture = new TestDatabaseFixture();
        var service = CreateService(fixture.Context);

        var publicId = Guid.NewGuid();
        var model = ValidMovieModel("Inception");
        model.PublicId = publicId;

        var result = await service.UpdateAsync(publicId, model);

        Assert.Equal(UpdateCatalogueResult.NotFound, result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsPublicIdMismatch_WhenRouteAndModelIdsDiffer()
    {
        await using var fixture = new TestDatabaseFixture();
        var service = CreateService(fixture.Context);

        var model = ValidMovieModel("Inception");
        model.PublicId = Guid.NewGuid();

        var result = await service.UpdateAsync(Guid.NewGuid(), model);

        Assert.Equal(UpdateCatalogueResult.PublicIdMismatch, result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsContentTypeChanged_WhenTypeIsChanged()
    {
        await using var fixture = new TestDatabaseFixture();

        var movie = ValidMovie("Inception");
        fixture.Context.CatalogueItems.Add(movie);
        await fixture.Context.SaveChangesAsync();

        var service = CreateService(fixture.Context);

        var model = ValidMovieModel("Inception");
        model.PublicId = movie.PublicId;
        model.ContentType = ContentTypes.Series;
        model.NumberOfSeasons = 1;
        model.TotalEpisodes = 10;

        var result = await service.UpdateAsync(movie.PublicId, model);

        Assert.Equal(UpdateCatalogueResult.ContentTypeChanged, result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingItem()
    {
        await using var fixture = new TestDatabaseFixture();

        var movie = ValidMovie("Inception");
        fixture.Context.CatalogueItems.Add(movie);
        await fixture.Context.SaveChangesAsync();

        var service = CreateService(fixture.Context);

        var result = await service.DeleteAsync(movie.PublicId);

        Assert.True(result);
        Assert.Empty(await fixture.Context.CatalogueItems.ToListAsync());
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenItemDoesNotExist()
    {
        await using var fixture = new TestDatabaseFixture();
        var service = CreateService(fixture.Context);

        var result = await service.DeleteAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task Seed_AddsInitialCatalogueItems()
    {
        await using var fixture = new TestDatabaseFixture();

        SeedData.Seed(fixture.Context);

        Assert.Equal(4, await fixture.Context.CatalogueItems.CountAsync());
        Assert.Equal(1, await fixture.Context.CatalogueItems.OfType<Movie>().CountAsync());
        Assert.Equal(1, await fixture.Context.CatalogueItems.OfType<Series>().CountAsync());
        Assert.Equal(1, await fixture.Context.CatalogueItems.OfType<Audiobook>().CountAsync());
        Assert.Equal(1, await fixture.Context.CatalogueItems.OfType<MusicAlbum>().CountAsync());
    }

    [Fact]
    public async Task Seed_DoesNotDuplicateItems_WhenCalledMoreThanOnce()
    {
        await using var fixture = new TestDatabaseFixture();

        SeedData.Seed(fixture.Context);
        SeedData.Seed(fixture.Context);

        Assert.Equal(4, await fixture.Context.CatalogueItems.CountAsync());
    }

    private static CatalogueService CreateService(AppDbContext context)
    {
        return new CatalogueService(context, new CatalogueItemMapper());
    }

    private static CatalogueFormViewModel ValidMovieModel(string title)
    {
        return new CatalogueFormViewModel
        {
            PublicId = Guid.NewGuid(),
            ContentType = ContentTypes.Movie,
            Title = title,
            Description = "Test description",
            ReleaseDate = new DateTime(2010, 7, 16),
            AgeRating = "12A",
            Genre = "Sci-Fi",
            DurationMinutes = 148,
            Director = "Christopher Nolan"
        };
    }

    private static Movie ValidMovie(string title)
    {
        return new Movie
        {
            PublicId = Guid.NewGuid(),
            Title = title,
            Description = "Test description",
            ReleaseDate = new DateTime(2010, 7, 16),
            AgeRating = "12A",
            Genre = "Sci-Fi",
            DurationMinutes = 148,
            Director = "Christopher Nolan"
        };
    }

    private static Series ValidSeries(string title)
    {
        return new Series
        {
            PublicId = Guid.NewGuid(),
            Title = title,
            Description = "Test description",
            ReleaseDate = new DateTime(2006, 3, 5),
            AgeRating = "PG",
            Genre = "Documentary",
            NumberOfSeasons = 1,
            TotalEpisodes = 11
        };
    }

    private static Audiobook ValidAudiobook(string title)
    {
        return new Audiobook
        {
            PublicId = Guid.NewGuid(),
            Title = title,
            Description = "Test description",
            ReleaseDate = new DateTime(2018, 10, 16),
            AgeRating = "PG",
            Genre = "Self-development",
            Author = "James Clear",
            Narrator = "James Clear",
            DurationMinutes = 320
        };
    }

    private static MusicAlbum ValidMusicAlbum(string title)
    {
        return new MusicAlbum
        {
            PublicId = Guid.NewGuid(),
            Title = title,
            Description = "Test description",
            ReleaseDate = new DateTime(2001, 3, 12),
            AgeRating = "PG",
            Genre = "Electronic",
            Artist = "Daft Punk",
            TrackCount = 14,
            RecordLabel = "Virgin"
        };
    }

    private sealed class TestDatabaseFixture : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;

        public TestDatabaseFixture()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            Context = new AppDbContext(options);
            Context.Database.EnsureCreated();
        }

        public AppDbContext Context { get; }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}