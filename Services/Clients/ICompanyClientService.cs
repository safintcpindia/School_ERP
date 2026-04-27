using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This interface defines the rules for how the application communicates with the main system to manage school company/branch information.
    /// </summary>
    public interface ICompanyClientService
    {
        /// <summary>
        /// Asks the main system for a list of all school companies.
        /// </summary>
        Task<ApiResponse<List<MstCompanyViewModel>>> GetAllAsync(bool includeDeleted = false);

        /// <summary>
        /// Asks the main system for details about a specific company using its ID.
        /// </summary>
        Task<ApiResponse<MstCompanyViewModel>> GetByIDAsync(int id);

        /// <summary>
        /// Sends information to the main system to either add a new company or update an existing one.
        /// </summary>
        Task<ApiResponse<dynamic>> UpsertAsync(MstCompanyUpsertRequest request);

        /// <summary>
        /// Tells the main system to remove a specific company.
        /// </summary>
        Task<ApiResponse<dynamic>> DeleteAsync(int id);

        /// <summary>
        /// Tells the main system to turn a company's active status on or off.
        /// </summary>
        Task<ApiResponse<dynamic>> ToggleStatusAsync(int id, bool isActive);
    }
}
