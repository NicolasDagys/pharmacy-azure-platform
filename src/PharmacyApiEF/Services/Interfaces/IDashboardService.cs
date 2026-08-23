using PharmacyApiEF.DTOs;

namespace PharmacyApiEF.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboard();
    }
}
