using StreamVault.Admin.ViewModels;

namespace StreamVault.Admin.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync();
}