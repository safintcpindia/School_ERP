using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface ILanguageClientService
    {
        Task<ApiResponse<List<MstLanguageViewModel>>> GetAllAsync(bool includeDeleted = false);
        Task<ApiResponse<MstLanguageViewModel>> GetByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertAsync(MstLanguageUpsertRequest request);
        Task<ApiResponse<dynamic>> DeleteAsync(int id);
        Task<ApiResponse<dynamic>> ToggleStatusAsync(int id, bool isActive);
    }
}
