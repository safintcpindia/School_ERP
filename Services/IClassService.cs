using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IClassService
    {
        List<MstClassViewModel> GetAllClasses(int companyId, int sessionId, bool includeDeleted = false);
        MstClassViewModel? GetClassByID(int classId);
        (bool success, string message) UpsertClass(MstClassUpsertRequest request, int companyId, int sessionId, int userId);
        (bool success, string message) DeleteClass(int classId, int userId);
        (bool success, string message) ToggleClassStatus(int classId, bool isActive, int userId);
    }
}
