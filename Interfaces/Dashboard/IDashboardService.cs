using RedBerryCorporate.DTOs.Dashboard;

namespace RedBerryCorporate.Interfaces.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardResponseDto> GetDashboardAsync();
    }
}