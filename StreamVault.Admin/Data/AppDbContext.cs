using Microsoft.EntityFrameworkCore;
using StreamVault.Admin.Models;

namespace StreamVault.Admin.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<CatalogueItem> CatalogueItems => Set<CatalogueItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogueItem>()
            .HasDiscriminator<string>("ContentType")
            .HasValue<Movie>(ContentTypes.Movie)
            .HasValue<Series>(ContentTypes.Series)
            .HasValue<Audiobook>(ContentTypes.Audiobook)
            .HasValue<MusicAlbum>(ContentTypes.MusicAlbum);

        modelBuilder.Entity<CatalogueItem>()
            .Property<string>("ContentType")
            .HasMaxLength(30);

        modelBuilder.Entity<CatalogueItem>()
            .Property(x => x.Title)
            .HasMaxLength(150)
            .IsRequired();

        modelBuilder.Entity<CatalogueItem>()
            .Property(x => x.Description)
            .HasMaxLength(1000);

        modelBuilder.Entity<CatalogueItem>()
            .Property(x => x.AgeRating)
            .HasMaxLength(20)
            .IsRequired();

        modelBuilder.Entity<CatalogueItem>()
            .Property(x => x.Genre)
            .HasMaxLength(80)
            .IsRequired();

        modelBuilder.Entity<CatalogueItem>()
            .Property(x => x.PublicId)
            .IsRequired();

        modelBuilder.Entity<CatalogueItem>()
            .HasIndex(x => x.PublicId)
            .IsUnique();

        modelBuilder.Entity<Movie>()
            .Property(x => x.DurationMinutes)
            .HasColumnName("DurationMinutes");

        modelBuilder.Entity<Audiobook>()
            .Property(x => x.DurationMinutes)
            .HasColumnName("DurationMinutes");

        base.OnModelCreating(modelBuilder);
    }
}
