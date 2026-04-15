using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface ISessionClientService
    {
        Task<ApiResponse<List<MstSessionViewModel>>> GetAllAsync(bool includeDeleted = false);
        Task<ApiResponse<MstSessionViewModel>> GetByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertAsync(MstSessionUpsertRequest request);
        Task<ApiResponse<dynamic>> DeleteAsync(int id);
        Task<ApiResponse<dynamic>> ToggleStatusAsync(int id, bool isActive);
    }
}
