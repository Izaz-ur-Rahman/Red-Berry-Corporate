using RedBerryCorporate.DTOs.Dashboard;

namespace RedBerryCorporate.Interfaces.Dashboard
{
    public interface IDashboardRepository
    {
        Task<DashboardResponseDto> GetDashboardAsync();
    }
}