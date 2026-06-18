using StreamVault.Admin.Models;

namespace StreamVault.Admin.Data;

public static class SeedData
{
    public static void Seed(AppDbContext context)
    {
        if (context.CatalogueItems.Any())
            return;

        context.CatalogueItems.AddRange(
            new Movie
            {
                Title = "Inception",
                Description = "A mind-bending sci-fi thriller.",
                ReleaseDate = new DateTime(2010, 7, 16),
                AgeRating = "12A",
                Genre = "Sci-Fi",
                DurationMinutes = 148,
                Director = "Christopher Nolan"
            },
            new Series
            {
                Title = "Planet Earth",
                Description = "Nature documentary series.",
                ReleaseDate = new DateTime(2006, 3, 5),
                AgeRating = "PG",
                Genre = "Documentary",
                NumberOfSeasons = 1,
                TotalEpisodes = 11
            },
            new Audiobook
            {
                Title = "Atomic Habits",
                Description = "A practical guide to building better habits.",
                ReleaseDate = new DateTime(2018, 10, 16),
                AgeRating = "PG",
                Genre = "Self-development",
                Author = "James Clear",
                Narrator = "James Clear",
                DurationMinutes = 320
            },
            new MusicAlbum
            {
                Title = "Random Access Memories",
                Description = "Electronic music album.",
                ReleaseDate = new DateTime(2013, 5, 17),
                AgeRating = "PG",
                Genre = "Electronic",
                Artist = "Daft Punk",
                TrackCount = 13,
                RecordLabel = "Columbia"
            }
        );

        context.SaveChanges();
    }
}