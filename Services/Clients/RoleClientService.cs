using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for RoleClientService.
    /// </summary>
    public class RoleClientService : BaseApiClient, IRoleClientService
    {
        public RoleClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<List<MstRoleViewModel>>> GetAllRolesAsync()
        {
            return await GetAsync<List<MstRoleViewModel>>("api/RoleApi");
        }

        public async Task<ApiResponse<MstRoleViewModel>> GetRoleByIdAsync(int id)
        {
            return await GetAsync<MstRoleViewModel>($"api/RoleApi/{id}");
        }

        public async Task<ApiResponse<int>> SaveRoleAsync(MstRoleUpsertRequest request)
        {
            return await PostAsync<int>("api/RoleApi/save", request);
        }

        public async Task<ApiResponse<bool>> ToggleStatusAsync(int roleId, bool isActive)
        {
            return await PostAsync<bool>($"api/RoleApi/toggle-status?roleId={roleId}&isActive={isActive}", null);
        }

        public async Task<ApiResponse<List<RoleMenuPermissionViewModel>>> GetPermissionsAsync(int roleId)
        {
            return await GetAsync<List<RoleMenuPermissionViewModel>>($"api/RoleApi/{roleId}/permissions");
        }

        public async Task<ApiResponse<bool>> SavePermissionsAsync(MstRolePermissionSaveRequest request)
        {
            return await PostAsync<bool>("api/RoleApi/save-permissions", request);
        }

        public async Task<ApiResponse<bool>> DeleteRoleAsync(int roleId)
        {
            return await DeleteAsync<bool>($"api/RoleApi/{roleId}");
        }
    }
}
