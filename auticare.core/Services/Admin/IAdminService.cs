using Auticare.Core.Models.Admin;

namespace Auticare.Services.Admin
{
    public interface IAdminService
    {
        // Dashboard
        Task<DashboardStats> GetDashboardStatsAsync();

        // Parents Management
        Task<PagedResponse<AdminParent>> GetParentsAsync(ParentFilter filter);
        Task<AdminParent?> GetParentByIdAsync(string id);
        Task<AdminParent> CreateParentAsync(CreateParentDto dto);
        Task<AdminParent?> UpdateParentAsync(string id, UpdateParentDto dto);
        Task<bool> DeleteParentAsync(string id);

        // Children Management
        Task<PagedResponse<AdminChild>> GetChildrenAsync(ChildFilter filter);
        Task<AdminChild?> GetChildByIdAsync(int id);
        Task<AdminChild> CreateChildAsync(CreateChildDto dto);
        Task<AdminChild?> UpdateChildAsync(int id, UpdateChildDto dto);
        Task<bool> DeleteChildAsync(int id);

        // Games Management
        Task<PagedResponse<AdminGame>> GetGamesAsync(GameFilter filter);
        Task<AdminGame?> GetGameByIdAsync(int id);
        Task<AdminGame> CreateGameAsync(CreateGameDto dto);
        Task<AdminGame?> UpdateGameAsync(int id, UpdateGameDto dto);
        Task<bool> DeleteGameAsync(int id);

        // Reports Management
        Task<PagedResponse<Auticare.Core.Models.Admin.ProgressReport>> GetReportsAsync(ReportFilter filter);
        Task<Auticare.Core.Models.Admin.ProgressReport?> GetReportByIdAsync(int id);

        // Messages Management
        Task<List<AdminMessage>> GetMessagesAsync();
        Task<AdminMessage> SendMessageAsync(CreateMessageDto dto);
    }
}
