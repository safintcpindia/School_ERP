using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IHumanResourceService
    {
        List<HRDesignationViewModel> GetAllDesignations(int companyId, int sessionId);
        HRDesignationViewModel? GetDesignationByID(int id);
        (bool Success, string Message) UpsertDesignation(HRDesignationUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteDesignation(int id, int userId);
        (bool Success, string Message) ToggleDesignationStatus(int id, bool isActive, int userId);

        List<HRDepartmentViewModel> GetAllDepartments(int companyId, int sessionId);
        HRDepartmentViewModel? GetDepartmentByID(int id);
        (bool Success, string Message) UpsertDepartment(HRDepartmentUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteDepartment(int id, int userId);
        (bool Success, string Message) ToggleDepartmentStatus(int id, bool isActive, int userId);

        List<HRLeaveTypeViewModel> GetAllLeaveTypes(int companyId, int sessionId);
        HRLeaveTypeViewModel? GetLeaveTypeByID(int id);
        (bool Success, string Message) UpsertLeaveType(HRLeaveTypeUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteLeaveType(int id, int userId);
        (bool Success, string Message) ToggleLeaveTypeStatus(int id, bool isActive, int userId);

        List<HRStaffViewModel> GetAllStaff(int companyId, int sessionId);
        HRStaffViewModel? GetStaffByID(int id);
        (bool Success, string Message) UpsertStaff(HRStaffUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteStaff(int id, int userId);
        string GetNewStaffCode(int companyId, int sessionId);
    }
}
