using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IRouteService
    {
        List<RouteViewModel> GetAllRoutes(int companyId, int sessionId);
        RouteViewModel? GetRouteByID(int id);
        (bool Success, string Message) UpsertRoute(RouteUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteRoute(int id, int userId);
        (bool Success, string Message) ToggleRouteStatus(int id, bool isActive, int userId);
    }
}
