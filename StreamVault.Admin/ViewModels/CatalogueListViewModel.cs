using StreamVault.Admin.Models;

namespace StreamVault.Admin.ViewModels;

public class CatalogueListViewModel
{
    public IReadOnlyList<CatalogueItem> Items { get; set; } = [];

    public string? Search { get; set; }

    public string? SelectedType { get; set; }
}