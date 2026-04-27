using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This interface defines the rules for managing user types, roles, and what parts of the system different users are allowed to see or do.
    /// </summary>
    public interface IUserManagementService
    {
        // --- User Types ---
        
        /// <summary>
        /// Gets a list of all categories of users (like 'Admin', 'Teacher', or 'Student').
        /// </summary>
        List<MstUserTypeViewModel> GetAllUserTypes();

        /// <summary>
        /// Finds the details of a specific user category using its ID.
        /// </summary>
        MstUserTypeViewModel? GetUserTypeByID(int userTypeID);

        /// <summary>
        /// Adds a new user category or updates an existing one.
        /// </summary>
        (bool success, string message) UpsertUserType(MstUserTypeUpsertRequest request, int userId, string ipAddress);

        /// <summary>
        /// Turns a user category on or off (e.g., to stop all 'Students' from logging in).
        /// </summary>
        (bool success, string message) ToggleUserTypeStatus(int userTypeID, bool isActive, int userId, string ipAddress);

        /// <summary>
        /// Removes a user category from the system.
        /// </summary>
        (bool success, string message) DeleteUserType(int userTypeID, int userId, string ipAddress);

        // --- Roles ---

        /// <summary>
        /// Gets a list of all defined roles in the system.
        /// </summary>
        List<MstRoleViewModel> GetAllRoles();

        /// <summary>
        /// Finds the details of a specific role using its ID.
        /// </summary>
        MstRoleViewModel? GetRoleByID(int roleID);

        /// <summary>
        /// Adds a new role or updates an existing one.
        /// </summary>
        (bool success, string message, int roleId) UpsertRole(MstRoleUpsertRequest request, int userId, string ipAddress);

        /// <summary>
        /// Turns a role on or off.
        /// </summary>
        (bool success, string message) ToggleRoleStatus(int roleID, bool isActive, int userId, string ipAddress);

        /// <summary>
        /// Removes a role from the system.
        /// </summary>
        (bool success, string message) DeleteRole(int roleID, int userId, string ipAddress);

        // --- Permissions ---

        /// <summary>
        /// Gets a list of all the different actions a user can take in the system.
        /// </summary>
        List<MstPermissionViewModel> GetAllPermissions();

        /// <summary>
        /// Finds the details of a specific action permission.
        /// </summary>
        MstPermissionViewModel? GetPermissionByID(int permissionID);

        /// <summary>
        /// Adds or updates an action permission.
        /// </summary>
        (bool success, string message) UpsertPermission(MstPermissionUpsertRequest request, int userId, string ipAddress);

        /// <summary>
        /// Turns a permission on or off.
        /// </summary>
        (bool success, string message) TogglePermissionStatus(int permissionID, bool isActive, int userId, string ipAddress);

        /// <summary>
        /// Removes a permission from the system.
        /// </summary>
        (bool success, string message) DeletePermission(int permissionID, int userId, string ipAddress);

        // --- Role Permission Tree ---

        /// <summary>
        /// Gets a grid showing which actions are allowed for a specific role.
        /// </summary>
        List<RoleMenuPermissionViewModel> GetPermissionsMatrix(int roleId);

        /// <summary>
        /// Saves the chosen list of allowed actions for a specific role.
        /// </summary>
        (bool success, string message) SaveRolePermissions(MstRolePermissionSaveRequest request, int adminId, string ipAddress);
        /// <summary>
        /// Gets the final list of everything a specific user is allowed to do.
        /// </summary>
        List<UserPermissionViewModel> GetUserPermissions(int userId);
    }
}
