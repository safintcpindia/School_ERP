using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface ISubjectService
    {
        List<MstSubjectViewModel> GetAllSubjects(int companyId, int sessionId, bool includeDeleted = false);
        MstSubjectViewModel? GetSubjectByID(int subjectId);
        (bool success, string message) UpsertSubject(MstSubjectUpsertRequest request, int companyId, int sessionId, int userId);
        (bool success, string message) DeleteSubject(int subjectId, int userId);
        (bool success, string message) ToggleSubjectStatus(int subjectId, bool isActive, int userId);
    }
}
