using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for LanguageClientService.
    /// </summary>
    public class LanguageClientService : BaseApiClient, ILanguageClientService
    {
        public LanguageClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<List<MstLanguageViewModel>>> GetAllAsync(bool includeDeleted = false)
        {
            return await GetAsync<List<MstLanguageViewModel>>($"api/LanguageApi/GetAll?includeDeleted={includeDeleted}");
        }

        public async Task<ApiResponse<MstLanguageViewModel>> GetByIDAsync(int id)
        {
            return await GetAsync<MstLanguageViewModel>($"api/LanguageApi/GetByID/{id}");
        }

        public async Task<ApiResponse<dynamic>> UpsertAsync(MstLanguageUpsertRequest request)
        {
            return await PostAsync<dynamic>("api/LanguageApi/Upsert", request);
        }

        public async Task<ApiResponse<dynamic>> DeleteAsync(int id)
        {
            return await PostAsync<dynamic>($"api/LanguageApi/Delete/{id}", null);
        }

        public async Task<ApiResponse<dynamic>> ToggleStatusAsync(int id, bool isActive)
        {
            return await PostAsync<dynamic>($"api/LanguageApi/ToggleStatus?id={id}&isActive={isActive}", null);
        }
    }
}
