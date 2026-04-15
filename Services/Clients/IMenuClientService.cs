using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface IMenuClientService
    {
        Task<ApiResponse<List<MenuViewModel>>> GetAllMenusAsync();
        Task<ApiResponse<MenuViewModel>> GetMenuByIdAsync(int id);
        Task<ApiResponse<bool>> SaveMenuAsync(MenuUpsertRequest request);
        Task<ApiResponse<bool>> ToggleStatusAsync(int menuId, bool isActive);
    }
}
