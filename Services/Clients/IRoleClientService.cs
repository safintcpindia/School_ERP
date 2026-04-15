using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface IRoleClientService
    {
        Task<ApiResponse<List<MstRoleViewModel>>> GetAllRolesAsync();
        Task<ApiResponse<MstRoleViewModel>> GetRoleByIdAsync(int id);
        Task<ApiResponse<int>> SaveRoleAsync(MstRoleUpsertRequest request);
        Task<ApiResponse<bool>> ToggleStatusAsync(int roleId, bool isActive);
        Task<ApiResponse<List<RoleMenuPermissionViewModel>>> GetPermissionsAsync(int roleId);
        Task<ApiResponse<bool>> SavePermissionsAsync(MstRolePermissionSaveRequest request);
    }
}
