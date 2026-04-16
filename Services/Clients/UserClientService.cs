using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for UserClientService.
    /// </summary>
    public class UserClientService : BaseApiClient, IUserClientService
    {
        public UserClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<List<UserViewModel>>> GetAllUsersAsync()
        {
            return await GetAsync<List<UserViewModel>>("api/user");
        }

        public async Task<ApiResponse<UserViewModel>> GetUserByIdAsync(int id)
        {
            return await GetAsync<UserViewModel>($"api/user/{id}");
        }

        public async Task<ApiResponse<List<int>>> GetUserRoleIdsAsync(int id)
        {
            return await GetAsync<List<int>>($"api/user/{id}/roles");
        }

        public async Task<ApiResponse<List<RoleViewModel>>> GetRolesDropdownAsync()
        {
            return await GetAsync<List<RoleViewModel>>("api/user/roles-dropdown");
        }

        public async Task<ApiResponse<List<MstUserTypeViewModel>>> GetUserTypesDropdownAsync()
        {
            return await GetAsync<List<MstUserTypeViewModel>>("api/user/types-dropdown");
        }

        public async Task<ApiResponse<bool>> SaveUserAsync(UserUpsertRequest request)
        {
            return await PostAsync<bool>("api/user/save", request);
        }

        public async Task<ApiResponse<bool>> ToggleStatusAsync(int userId, bool isActive)
        {
            return await PostAsync<bool>($"api/user/toggle-status?userId={userId}&isActive={isActive}", null);
        }

        public async Task<ApiResponse<bool>> UnlockUserAsync(int id)
        {
            return await PostAsync<bool>($"api/user/unlock/{id}", null);
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
        {
            return await PostAsync<bool>($"api/user/delete/{id}", null);
        }
    }
}
