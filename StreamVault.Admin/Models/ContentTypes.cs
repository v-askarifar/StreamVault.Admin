namespace StreamVault.Admin.Models;

public static class ContentTypes
{
    public const string Movie = "Movie";
    public const string Series = "Series";
    public const string Audiobook = "Audiobook";
    public const string MusicAlbum = "MusicAlbum";

    public static readonly string[] All =
    [
        Movie,
        Series,
        Audiobook,
        MusicAlbum
    ];

    public static bool IsValid(string? contentType)
    {
        return All.Contains(contentType);
    }

    public static string GetDisplayName(string? contentType)
    {
        return contentType switch
        {
            Movie => "Movie",
            Series => "Series",
            Audiobook => "Audiobook",
            MusicAlbum => "Music Album",
            _ => "Unknown"
        };
    }
}
