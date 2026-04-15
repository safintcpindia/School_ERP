using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public class CompanyClientService : BaseApiClient, ICompanyClientService
    {
        public CompanyClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<List<MstCompanyViewModel>>> GetAllAsync(bool includeDeleted = false)
        {
            return await GetAsync<List<MstCompanyViewModel>>($"api/CompanyApi/GetAll?includeDeleted={includeDeleted}");
        }

        public async Task<ApiResponse<MstCompanyViewModel>> GetByIDAsync(int id)
        {
            return await GetAsync<MstCompanyViewModel>($"api/CompanyApi/GetByID/{id}");
        }

        public async Task<ApiResponse<dynamic>> UpsertAsync(MstCompanyUpsertRequest request)
        {
            return await PostAsync<dynamic>("api/CompanyApi/Upsert", request);
        }

        public async Task<ApiResponse<dynamic>> DeleteAsync(int id)
        {
            return await PostAsync<dynamic>($"api/CompanyApi/Delete/{id}", null);
        }

        public async Task<ApiResponse<dynamic>> ToggleStatusAsync(int id, bool isActive)
        {
            return await PostAsync<dynamic>($"api/CompanyApi/ToggleStatus?id={id}&isActive={isActive}", null);
        }
    }
}
