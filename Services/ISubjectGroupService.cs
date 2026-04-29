using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface ISubjectGroupService
    {
        List<MstSubjectGroupViewModel> GetAll(int companyId, int sessionId, bool includeDeleted = false);
        MstSubjectGroupViewModel? GetByID(int id);
        (bool success, string message) Upsert(MstSubjectGroupUpsertRequest request, int companyId, int sessionId, int userId);
        (bool success, string message) Delete(int id, int userId);
        (bool success, string message) ToggleStatus(int id, bool isActive, int userId);
    }
}
