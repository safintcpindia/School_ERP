using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for OrganisationClientService.
    /// </summary>
    public class OrganisationClientService : BaseApiClient, IOrganisationClientService
    {
        public OrganisationClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<List<OrganisationViewModel>>> GetAllOrganisationsAsync(bool includeDeleted = false)
        {
            return await GetAsync<List<OrganisationViewModel>>($"api/OrganisationApi/GetAll?includeDeleted={includeDeleted}");
        }

        public async Task<ApiResponse<OrganisationViewModel>> GetOrganisationByIDAsync(int id)
        {
            return await GetAsync<OrganisationViewModel>($"api/OrganisationApi/GetByID/{id}");
        }

        public async Task<ApiResponse<dynamic>> UpsertOrganisationAsync(OrganisationUpsertRequest request)
        {
            return await PostAsync<dynamic>("api/OrganisationApi/Upsert", request);
        }

        public async Task<ApiResponse<dynamic>> DeleteOrganisationAsync(int id)
        {
            return await PostAsync<dynamic>($"api/OrganisationApi/Delete/{id}", null);
        }

        public async Task<ApiResponse<dynamic>> ToggleStatusAsync(int id, bool isActive)
        {
            return await PostAsync<dynamic>($"api/OrganisationApi/ToggleStatus?id={id}&isActive={isActive}", null);
        }
    }
}
