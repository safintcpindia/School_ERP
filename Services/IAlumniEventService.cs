using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IAlumniEventService
    {
        List<AlumniEventViewModel> GetEvents(string? searchText, int companyId);
        (bool Success, string Message) UpsertEvent(AlumniEventUpsertRequest req, int companyId, int userId);
        (bool Success, string Message) DeleteEvent(int eventId, int companyId);
        (bool Success, string Message) ToggleEventStatus(int eventId, bool isActive, int companyId);
        (byte[]? Bytes, string? FileName, string? ContentType) GetEventPhoto(int eventId, int companyId);
    }
}
