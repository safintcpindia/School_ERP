using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface IUserTypeClientService
    {
        Task<ApiResponse<List<MstUserTypeViewModel>>> GetAllAsync();
        Task<ApiResponse<MstUserTypeViewModel>> GetByIdAsync(int id);
        Task<ApiResponse<bool>> SaveAsync(MstUserTypeUpsertRequest request);
        Task<ApiResponse<bool>> ToggleStatusAsync(int typeId, bool isActive);
    }
}
