using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IAccountEntryService
    {
        List<AccountEntryViewModel> GetAllAccountEntries(int companyId, int sessionId, string entryType, bool includeDeleted = false);
        List<AccountEntryViewModel> SearchAccountEntries(int companyId, int sessionId, string entryType, string searchType, string? dateFrom, string? dateTo);
        AccountEntryViewModel? GetAccountEntryByID(int id);
        (bool Success, string Message) UpsertAccountEntry(AccountEntryUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteAccountEntry(int id, int userId);
        (bool Success, string Message) ToggleAccountEntryStatus(int id, bool isActive, int userId);
    }
}
