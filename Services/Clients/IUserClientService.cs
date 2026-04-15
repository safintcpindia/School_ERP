using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface IUserClientService
    {
        Task<ApiResponse<List<UserViewModel>>> GetAllUsersAsync();
        Task<ApiResponse<UserViewModel>> GetUserByIdAsync(int id);
        Task<ApiResponse<List<int>>> GetUserRoleIdsAsync(int id);
        Task<ApiResponse<List<RoleViewModel>>> GetRolesDropdownAsync();
        Task<ApiResponse<List<MstUserTypeViewModel>>> GetUserTypesDropdownAsync();
        Task<ApiResponse<bool>> SaveUserAsync(UserUpsertRequest request);
        Task<ApiResponse<bool>> ToggleStatusAsync(int userId, bool isActive);
        Task<ApiResponse<bool>> UnlockUserAsync(int id);
    }
}
