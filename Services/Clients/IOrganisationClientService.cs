using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface IOrganisationClientService
    {
        Task<ApiResponse<List<OrganisationViewModel>>> GetAllOrganisationsAsync(bool includeDeleted = false);
        Task<ApiResponse<OrganisationViewModel>> GetOrganisationByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertOrganisationAsync(OrganisationUpsertRequest request);
        Task<ApiResponse<dynamic>> DeleteOrganisationAsync(int id);
        Task<ApiResponse<dynamic>> ToggleStatusAsync(int id, bool isActive);
    }
}
