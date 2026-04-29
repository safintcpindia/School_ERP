using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IVehicleAssignService
    {
        List<VehicleAssignViewModel> GetAllAssignments(int companyId, int sessionId);
        VehicleAssignViewModel? GetAssignmentByID(int id);
        (bool Success, string Message) UpsertAssignments(VehicleAssignUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteAssignment(int id, int userId);
        (bool Success, string Message) ToggleAssignmentStatus(int id, bool isActive, int userId);
    }
}
