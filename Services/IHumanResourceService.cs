using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// Interface for managing human resources, including staff, departments, designations, and leave types.
    /// </summary>
    public interface IHumanResourceService
    {
        // --- Designation ---

        /// <summary>
        /// Retrieves a list of all job designations (e.g., 'Teacher', 'Accountant') from the database.
        /// </summary>
        List<HRDesignationViewModel> GetAllDesignations(int companyId, int sessionId);

        /// <summary>
        /// Finds and returns the details of a specific designation using its unique ID.
        /// </summary>
        HRDesignationViewModel? GetDesignationByID(int id);

        /// <summary>
        /// Adds a new designation or updates an existing one in the database.
        /// </summary>
        (bool Success, string Message) UpsertDesignation(HRDesignationUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a designation from the system.
        /// </summary>
        (bool Success, string Message) DeleteDesignation(int id, int userId);

        /// <summary>
        /// Turns a designation's active status on or off.
        /// </summary>
        (bool Success, string Message) ToggleDesignationStatus(int id, bool isActive, int userId);

        // --- Department ---

        /// <summary>
        /// Retrieves a list of all school departments (e.g., 'Academic', 'Admin') from the database.
        /// </summary>
        List<HRDepartmentViewModel> GetAllDepartments(int companyId, int sessionId);

        /// <summary>
        /// Finds and returns the details of a specific department using its unique ID.
        /// </summary>
        HRDepartmentViewModel? GetDepartmentByID(int id);

        /// <summary>
        /// Adds a new department or updates an existing one in the database.
        /// </summary>
        (bool Success, string Message) UpsertDepartment(HRDepartmentUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a department from the system.
        /// </summary>
        (bool Success, string Message) DeleteDepartment(int id, int userId);

        /// <summary>
        /// Turns a department's active status on or off.
        /// </summary>
        (bool Success, string Message) ToggleDepartmentStatus(int id, bool isActive, int userId);

        // --- Leave Type ---

        /// <summary>
        /// Retrieves a list of all leave categories (e.g., 'Sick Leave', 'Casual Leave') from the database.
        /// </summary>
        List<HRLeaveTypeViewModel> GetAllLeaveTypes(int companyId, int sessionId);

        /// <summary>
        /// Finds and returns the details of a specific leave type using its unique ID.
        /// </summary>
        HRLeaveTypeViewModel? GetLeaveTypeByID(int id);

        /// <summary>
        /// Adds a new leave type or updates an existing one in the database.
        /// </summary>
        (bool Success, string Message) UpsertLeaveType(HRLeaveTypeUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a leave type from the system.
        /// </summary>
        (bool Success, string Message) DeleteLeaveType(int id, int userId);

        /// <summary>
        /// Turns a leave type's active status on or off.
        /// </summary>
        (bool Success, string Message) ToggleLeaveTypeStatus(int id, bool isActive, int userId);

        // --- Staff ---

        /// <summary>
        /// Retrieves a complete list of all staff members from the database.
        /// </summary>
        List<HRStaffViewModel> GetAllStaff(int companyId, int sessionId);

        /// <summary>
        /// Finds and returns the details of a specific staff member using its unique ID.
        /// </summary>
        HRStaffViewModel? GetStaffByID(int id);

        /// <summary>
        /// Adds a new staff member or updates an existing one, including personal details, documents, and system access.
        /// </summary>
        (bool Success, string Message) UpsertStaff(HRStaffUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a staff member from the system.
        /// </summary>
        (bool Success, string Message) DeleteStaff(int id, int userId);

        /// <summary>
        /// Generates a unique code for a new staff member.
        /// </summary>
        string GetNewStaffCode(int companyId, int sessionId);
    }
}
