using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface ILanguageService
    {
        List<MstLanguageViewModel> GetAllLanguages(bool includeDeleted = false);
        MstLanguageViewModel? GetLanguageByID(int languageId);
        (bool success, string message) UpsertLanguage(MstLanguageUpsertRequest request, int userId);
        (bool success, string message) DeleteLanguage(int languageId, int userId);
        (bool success, string message) ToggleLanguageStatus(int languageId, bool isActive, int userId);
    }
}
