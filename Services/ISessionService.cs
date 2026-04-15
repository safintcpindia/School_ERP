using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface ISessionService
    {
        List<MstSessionViewModel> GetAllSessions(bool includeDeleted = false);
        MstSessionViewModel? GetSessionByID(int sessionId);
        (bool success, string message) UpsertSession(MstSessionUpsertRequest request, int userId);
        (bool success, string message) DeleteSession(int sessionId, int userId);
        (bool success, string message) ToggleSessionStatus(int sessionId, bool isActive, int userId);
    }
}
