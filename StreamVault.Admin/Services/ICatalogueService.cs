using StreamVault.Admin.Models;
using StreamVault.Admin.ViewModels;

namespace StreamVault.Admin.Services;

public interface ICatalogueService
{
    Task<CatalogueListViewModel> GetListAsync(string? type, string? search);
    Task<CatalogueFormViewModel?> GetForEditAsync(Guid publicId);
    Task CreateAsync(CatalogueFormViewModel model);
    Task<UpdateCatalogueResult> UpdateAsync(Guid publicId, CatalogueFormViewModel model);
    Task<bool> DeleteAsync(Guid publicId);
}

public enum UpdateCatalogueResult
{
    Success,
    NotFound,
    PublicIdMismatch,
    ContentTypeChanged
}
