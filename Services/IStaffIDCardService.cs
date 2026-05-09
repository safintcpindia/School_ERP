using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IStaffIDCardService
    {
        List<StaffIDCardViewModel> GetAll(int companyId, int sessionId);
        StaffIDCardViewModel GetByID(int id);
        (int Result, string Message) Upsert(StaffIDCardUpsertRequest request, int userId, int companyId, int sessionId);
        (int Result, string Message) Delete(int id, int userId);
        (int Result, string Message) ToggleStatus(int id, bool isActive, int userId);
    }
}
