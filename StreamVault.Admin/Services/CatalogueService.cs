using Microsoft.EntityFrameworkCore;
using StreamVault.Admin.Data;
using StreamVault.Admin.Models;
using StreamVault.Admin.ViewModels;

namespace StreamVault.Admin.Services;

public class CatalogueService : ICatalogueService
{
    private readonly AppDbContext _context;
    private readonly CatalogueItemMapper _mapper;

    public CatalogueService(AppDbContext context, CatalogueItemMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CatalogueListViewModel> GetListAsync(string? type, string? search)
    {
        var query = _context.CatalogueItems.AsNoTracking().AsQueryable();

        var trimmedSearch = search?.Trim();

        if (!string.IsNullOrWhiteSpace(trimmedSearch))
        {
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{trimmedSearch}%"));
        }

        query = ApplyTypeFilter(query, type);

        return new CatalogueListViewModel
        {
            Items = await query.OrderBy(x => x.Title).ToListAsync(),
            Search = trimmedSearch,
            SelectedType = ContentTypes.IsValid(type) ? type : null
        };
    }

    public async Task<CatalogueFormViewModel?> GetForEditAsync(Guid publicId)
    {
        var item = await _context.CatalogueItems.FirstOrDefaultAsync(x => x.PublicId == publicId);
        return item is null ? null : _mapper.ToFormModel(item);
    }

    public async Task CreateAsync(CatalogueFormViewModel model)
    {
        var item = _mapper.ToEntity(model);
        _context.CatalogueItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task<UpdateCatalogueResult> UpdateAsync(Guid publicId, CatalogueFormViewModel model)
    {
        if (publicId != model.PublicId)
            return UpdateCatalogueResult.PublicIdMismatch;

        var existing = await _context.CatalogueItems.FirstOrDefaultAsync(x => x.PublicId == publicId);

        if (existing is null)
            return UpdateCatalogueResult.NotFound;

        if (model.ContentType != existing.ContentTypeKey)
            return UpdateCatalogueResult.ContentTypeChanged;

        _mapper.UpdateEntity(model, existing);
        await _context.SaveChangesAsync();

        return UpdateCatalogueResult.Success;
    }

    public async Task<bool> DeleteAsync(Guid publicId)
    {
        var item = await _context.CatalogueItems.FirstOrDefaultAsync(x => x.PublicId == publicId);

        if (item is null)
            return false;

        _context.CatalogueItems.Remove(item);
        await _context.SaveChangesAsync();

        return true;
    }

    private static IQueryable<CatalogueItem> ApplyTypeFilter(IQueryable<CatalogueItem> query, string? type)
    {
        return type switch
        {
            ContentTypes.Movie => query.OfType<Movie>(),
            ContentTypes.Series => query.OfType<Series>(),
            ContentTypes.Audiobook => query.OfType<Audiobook>(),
            ContentTypes.MusicAlbum => query.OfType<MusicAlbum>(),
            _ => query
        };
    }
}
