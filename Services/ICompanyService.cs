using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface ICompanyService
    {
        List<MstCompanyViewModel> GetAllCompanies(bool includeDeleted = false);
        MstCompanyViewModel? GetCompanyByID(int companyId);
        (bool success, string message) UpsertCompany(MstCompanyUpsertRequest request, int userId);
        (bool success, string message) DeleteCompany(int companyId, int userId);
        (bool success, string message) ToggleStatus(int companyId, bool isActive, int userId);
    }
}
