using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface ISectionService
    {
        List<MstSectionViewModel> GetAllSections(int companyId, int sessionId, bool includeDeleted = false);
        List<MstSectionViewModel> GetSectionsByClass(int classId);
        MstSectionViewModel? GetSectionByID(int sectionId);
        (bool success, string message) UpsertSection(MstSectionUpsertRequest request, int companyId, int sessionId, int userId);
        (bool success, string message) DeleteSection(int sectionId, int userId);
        (bool success, string message) ToggleSectionStatus(int sectionId, bool isActive, int userId);
    }
}
