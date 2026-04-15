using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for MenuClientService.
    /// </summary>
    public class MenuClientService : BaseApiClient, IMenuClientService
    {
        public MenuClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<List<MenuViewModel>>> GetAllMenusAsync()
        {
            return await GetAsync<List<MenuViewModel>>("api/MasterMenuApi");
        }

        public async Task<ApiResponse<MenuViewModel>> GetMenuByIdAsync(int id)
        {
            return await GetAsync<MenuViewModel>($"api/MasterMenuApi/{id}");
        }

        public async Task<ApiResponse<bool>> SaveMenuAsync(MenuUpsertRequest request)
        {
            return await PostAsync<bool>("api/MasterMenuApi/save", request);
        }

        public async Task<ApiResponse<bool>> ToggleStatusAsync(int menuId, bool isActive)
        {
            return await PostAsync<bool>($"api/MasterMenuApi/toggle-status?menuId={menuId}&isActive={isActive}", null);
        }
    }
}
