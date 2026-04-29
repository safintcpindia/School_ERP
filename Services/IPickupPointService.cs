using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IPickupPointService
    {
        List<PickupPointViewModel> GetAllPickupPoints(int companyId, int sessionId);
        PickupPointViewModel? GetPickupPointByID(int id);
        (bool Success, string Message) UpsertPickupPoint(PickupPointUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeletePickupPoint(int id, int userId);
        (bool Success, string Message) TogglePickupPointStatus(int id, bool isActive, int userId);
    }
}
