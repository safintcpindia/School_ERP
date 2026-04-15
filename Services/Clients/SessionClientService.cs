using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for SessionClientService.
    /// </summary>
    public class SessionClientService : BaseApiClient, ISessionClientService
    {
        public SessionClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<List<MstSessionViewModel>>> GetAllAsync(bool includeDeleted = false)
        {
            return await GetAsync<List<MstSessionViewModel>>($"api/SessionApi/GetAll?includeDeleted={includeDeleted}");
        }

        public async Task<ApiResponse<MstSessionViewModel>> GetByIDAsync(int id)
        {
            return await GetAsync<MstSessionViewModel>($"api/SessionApi/GetByID/{id}");
        }

        public async Task<ApiResponse<dynamic>> UpsertAsync(MstSessionUpsertRequest request)
        {
            return await PostAsync<dynamic>("api/SessionApi/Upsert", request);
        }

        public async Task<ApiResponse<dynamic>> DeleteAsync(int id)
        {
            return await PostAsync<dynamic>($"api/SessionApi/Delete/{id}", null);
        }

        public async Task<ApiResponse<dynamic>> ToggleStatusAsync(int id, bool isActive)
        {
            return await PostAsync<dynamic>($"api/SessionApi/ToggleStatus?id={id}&isActive={isActive}", null);
        }
    }
}
