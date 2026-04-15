using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface ICurrencyClientService
    {
        Task<ApiResponse<List<MstCurrencyViewModel>>> GetAllAsync(bool includeDeleted = false);
        Task<ApiResponse<MstCurrencyViewModel>> GetByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertAsync(MstCurrencyUpsertRequest request);
        Task<ApiResponse<dynamic>> DeleteAsync(int id);
        Task<ApiResponse<dynamic>> ToggleStatusAsync(int id, bool isActive);
    }
}
