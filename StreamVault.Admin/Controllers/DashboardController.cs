using Microsoft.AspNetCore.Mvc;
using StreamVault.Admin.Services;

namespace StreamVault.Admin.Controllers;

public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _dashboardService.GetDashboardAsync();

        return View(model);
    }
}