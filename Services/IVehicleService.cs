using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IVehicleService
    {
        List<VehicleViewModel> GetAllVehicles(int companyId, int sessionId);
        VehicleViewModel? GetVehicleByID(int id);
        (bool Success, string Message) UpsertVehicle(VehicleUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteVehicle(int id, int userId);
        (bool Success, string Message) ToggleVehicleStatus(int id, bool isActive, int userId);
    }
}
