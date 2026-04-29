using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IFieldService
    {
        List<FieldModel> GetAllFields(int companyId, int sessionId, bool? isSystemField = null, string belongsTo = null);
        FieldModel GetFieldByID(int id);
        (bool Success, string Message) UpsertField(FieldViewModel model, int userId);
        (bool Success, string Message) DeleteField(int id, int userId);
        (bool Success, string Message) ToggleFieldStatus(int id, bool isActive, int userId);
        List<IDAutoGenSettings> GetIDAutoGenSettings(int companyId, int sessionId);
        (bool Success, string Message) SaveIDAutoGenSettings(IDAutoGenRequest request, int userId);
    }
}
