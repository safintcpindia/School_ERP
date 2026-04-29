using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IRoutePickupPointService
    {
        List<RoutePickupPointViewModel> GetAllRoutePickupPoints(int companyId, int sessionId);
        RoutePickupPointViewModel? GetRoutePickupPointByID(int id);
        (bool Success, string Message) UpsertRoutePickupPoint(RoutePickupPointUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteRoutePickupPoint(int id, int userId);
        (bool Success, string Message) ToggleRoutePickupPointStatus(int id, bool isActive, int userId);
    }
}
