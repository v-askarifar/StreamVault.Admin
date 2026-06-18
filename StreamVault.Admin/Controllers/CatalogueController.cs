using Microsoft.AspNetCore.Mvc;
using StreamVault.Admin.Services;
using StreamVault.Admin.ViewModels;

namespace StreamVault.Admin.Controllers;

public class CatalogueController : Controller
{
    private readonly ICatalogueService _catalogueService;

    public CatalogueController(ICatalogueService catalogueService)
    {
        _catalogueService = catalogueService;
    }

    public async Task<IActionResult> Index(string? type, string? search)
    {
        var model = await _catalogueService.GetListAsync(type, search);
        return View(model);
    }

    public IActionResult Create()
    {
        return View(new CatalogueFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CatalogueFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _catalogueService.CreateAsync(model);

        TempData["SuccessMessage"] = "Content item created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid publicId)
    {
        var model = await _catalogueService.GetForEditAsync(publicId);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid publicId, CatalogueFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _catalogueService.UpdateAsync(publicId, model);

        if (result == UpdateCatalogueResult.Success)
        {
            TempData["SuccessMessage"] = "Content item updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        if (result == UpdateCatalogueResult.NotFound)
            return NotFound();

        if (result == UpdateCatalogueResult.PublicIdMismatch)
            return BadRequest();

        ModelState.AddModelError(nameof(model.ContentType), "Content type cannot be changed while editing.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid publicId)
    {
        var deleted = await _catalogueService.DeleteAsync(publicId);

        if (!deleted)
            return NotFound();

        TempData["SuccessMessage"] = "Content item deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
