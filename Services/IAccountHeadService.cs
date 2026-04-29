using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IAccountHeadService
    {
        List<AccountHeadViewModel> GetAllAccountHeads(int companyId, int sessionId, string headType, bool includeDeleted = false);
        AccountHeadViewModel? GetAccountHeadByID(int id);
        (bool Success, string Message) UpsertAccountHead(AccountHeadUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteAccountHead(int id, int userId);
        (bool Success, string Message) ToggleAccountHeadStatus(int id, bool isActive, int userId);
    }
}
