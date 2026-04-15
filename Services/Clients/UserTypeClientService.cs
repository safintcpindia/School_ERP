using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for UserTypeClientService.
    /// </summary>
    public class UserTypeClientService : BaseApiClient, IUserTypeClientService
    {
        public UserTypeClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<List<MstUserTypeViewModel>>> GetAllAsync()
        {
            return await GetAsync<List<MstUserTypeViewModel>>("api/UserTypeApi");
        }

        public async Task<ApiResponse<MstUserTypeViewModel>> GetByIdAsync(int id)
        {
            return await GetAsync<MstUserTypeViewModel>($"api/UserTypeApi/{id}");
        }

        public async Task<ApiResponse<bool>> SaveAsync(MstUserTypeUpsertRequest request)
        {
            return await PostAsync<bool>("api/UserTypeApi/save", request);
        }

        public async Task<ApiResponse<bool>> ToggleStatusAsync(int typeId, bool isActive)
        {
            return await PostAsync<bool>($"api/UserTypeApi/toggle-status?typeId={typeId}&isActive={isActive}", null);
        }
    }
}
