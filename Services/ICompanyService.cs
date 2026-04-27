using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This interface defines the rules for managing school companies or branches.
    /// </summary>
    public interface ICompanyService
    {
        /// <summary>
        /// Gets a list of all companies registered in the system.
        /// </summary>
        List<MstCompanyViewModel> GetAllCompanies(bool includeDeleted = false);

        /// <summary>
        /// Finds and returns the details of a specific company using its ID number.
        /// </summary>
        MstCompanyViewModel? GetCompanyByID(int companyId);

        /// <summary>
        /// Adds a new company or updates an existing one with new information.
        /// </summary>
        (bool success, string message) UpsertCompany(MstCompanyUpsertRequest request, int userId);

        /// <summary>
        /// Removes a company from the system.
        /// </summary>
        (bool success, string message) DeleteCompany(int companyId, int userId);

        /// <summary>
        /// Turns a company's active status on or off (e.g., to temporarily disable it).
        /// </summary>
        (bool success, string message) ToggleStatus(int companyId, bool isActive, int userId);
    }
}
