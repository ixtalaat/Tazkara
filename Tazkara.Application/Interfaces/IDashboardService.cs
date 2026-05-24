using Tazkara.Application.DTOs.Dashboard;
using Tazkara.Application.Wrappers;

namespace Tazkara.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<ApiResponse<OrganizerDashboardDto>> GetOrganizerDashboardAsync(Guid organizerId);
    }
}
