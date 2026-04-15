using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface ICompanyClientService
    {
        Task<ApiResponse<List<MstCompanyViewModel>>> GetAllAsync(bool includeDeleted = false);
        Task<ApiResponse<MstCompanyViewModel>> GetByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertAsync(MstCompanyUpsertRequest request);
        Task<ApiResponse<dynamic>> DeleteAsync(int id);
        Task<ApiResponse<dynamic>> ToggleStatusAsync(int id, bool isActive);
    }
}
