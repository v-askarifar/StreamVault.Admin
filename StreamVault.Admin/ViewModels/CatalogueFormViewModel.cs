using System.ComponentModel.DataAnnotations;
using StreamVault.Admin.Models;

namespace StreamVault.Admin.ViewModels;

public class CatalogueFormViewModel : IValidatableObject
{
    public Guid PublicId { get; set; }

    public int Id { get; set; }

    [Required(ErrorMessage = "Content type is required.")]
    public string ContentType { get; set; } = ContentTypes.Movie;

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(150, ErrorMessage = "Title must be 150 characters or fewer.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description must be 1000 characters or fewer.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Release date is required.")]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Age rating is required.")]
    [StringLength(20, ErrorMessage = "Age rating must be 20 characters or fewer.")]
    public string AgeRating { get; set; } = string.Empty;

    [Required(ErrorMessage = "Genre is required.")]
    [StringLength(80, ErrorMessage = "Genre must be 80 characters or fewer.")]
    public string Genre { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "Duration must be positive.")]
    public int? DurationMinutes { get; set; }

    [StringLength(120, ErrorMessage = "Director must be 120 characters or fewer.")]
    public string? Director { get; set; }

    [Range(1, 100, ErrorMessage = "Number of seasons must be between 1 and 100.")]
    public int? NumberOfSeasons { get; set; }

    [Range(1, 10000, ErrorMessage = "Total episodes must be between 1 and 10000.")]
    public int? TotalEpisodes { get; set; }

    [StringLength(120, ErrorMessage = "Author must be 120 characters or fewer.")]
    public string? Author { get; set; }

    [StringLength(120, ErrorMessage = "Narrator must be 120 characters or fewer.")]
    public string? Narrator { get; set; }

    [StringLength(120, ErrorMessage = "Artist must be 120 characters or fewer.")]
    public string? Artist { get; set; }

    [Range(1, 500, ErrorMessage = "Track count must be between 1 and 500.")]
    public int? TrackCount { get; set; }

    [StringLength(120, ErrorMessage = "Record label must be 120 characters or fewer.")]
    public string? RecordLabel { get; set; }

    public string ContentTypeDisplayName => ContentTypes.GetDisplayName(ContentType);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!ContentTypes.IsValid(ContentType))
        {
            yield return new ValidationResult("Invalid content type.", [nameof(ContentType)]);
            yield break;
        }

        foreach (var result in ValidateMovie())
            yield return result;

        foreach (var result in ValidateSeries())
            yield return result;

        foreach (var result in ValidateAudiobook())
            yield return result;

        foreach (var result in ValidateMusicAlbum())
            yield return result;
    }

    private IEnumerable<ValidationResult> ValidateMovie()
    {
        if (ContentType != ContentTypes.Movie)
            yield break;

        if (DurationMinutes is null or <= 0)
            yield return new ValidationResult("Movie duration must be positive.", [nameof(DurationMinutes)]);

        if (string.IsNullOrWhiteSpace(Director))
            yield return new ValidationResult("Director is required for movies.", [nameof(Director)]);
    }

    private IEnumerable<ValidationResult> ValidateSeries()
    {
        if (ContentType != ContentTypes.Series)
            yield break;

        if (NumberOfSeasons is null or <= 0)
            yield return new ValidationResult("Number of seasons must be positive.", [nameof(NumberOfSeasons)]);

        if (TotalEpisodes is null or <= 0)
            yield return new ValidationResult("Total episodes must be positive.", [nameof(TotalEpisodes)]);

        if (NumberOfSeasons.HasValue && TotalEpisodes.HasValue && TotalEpisodes < NumberOfSeasons)
        {
            yield return new ValidationResult(
                "Total episodes must be greater than or equal to number of seasons.",
                [nameof(TotalEpisodes)]);
        }
    }

    private IEnumerable<ValidationResult> ValidateAudiobook()
    {
        if (ContentType != ContentTypes.Audiobook)
            yield break;

        if (string.IsNullOrWhiteSpace(Author))
            yield return new ValidationResult("Author is required for audiobooks.", [nameof(Author)]);

        if (string.IsNullOrWhiteSpace(Narrator))
            yield return new ValidationResult("Narrator is required for audiobooks.", [nameof(Narrator)]);

        if (DurationMinutes is null or <= 0)
            yield return new ValidationResult("Audiobook duration must be positive.", [nameof(DurationMinutes)]);
    }

    private IEnumerable<ValidationResult> ValidateMusicAlbum()
    {
        if (ContentType != ContentTypes.MusicAlbum)
            yield break;

        if (string.IsNullOrWhiteSpace(Artist))
            yield return new ValidationResult("Artist is required for music albums.", [nameof(Artist)]);

        if (TrackCount is null or <= 0)
            yield return new ValidationResult("Track count must be positive.", [nameof(TrackCount)]);
    }
}
