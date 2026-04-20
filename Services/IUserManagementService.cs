using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// Provides an abstraction layer for managing user types, roles, and access permissions.
    /// This service handles all business logic and data access procedures regarding the identity modules.
    /// </summary>
    public interface IUserManagementService
    {
        // --- User Types ---
        
        /// <summary>
        /// Retrieves a complete list of all active and inactive user types in the system.
        /// </summary>
        /// <returns>A collection of User Type View Model objects.</returns>
        List<MstUserTypeViewModel> GetAllUserTypes();

        /// <summary>
        /// Retrieves a specific user type by its unique identifier.
        /// </summary>
        /// <param name="userTypeID">The unique identifier of the user type.</param>
        /// <returns>The User Type View Model, or null if not found.</returns>
        MstUserTypeViewModel? GetUserTypeByID(int userTypeID);

        /// <summary>
        /// Inserts a new user type or updates an existing user type based on the provided request model.
        /// </summary>
        /// <param name="request">The data payload containing User Type details.</param>
        /// <param name="userId">The ID of the user performing this operation.</param>
        /// <param name="ipAddress">The IP Address of the user (for auditing).</param>
        /// <returns>A tuple containing the success status and a string message.</returns>
        (bool success, string message) UpsertUserType(MstUserTypeUpsertRequest request, int userId, string ipAddress);

        /// <summary>
        /// Toggles the active status of a user type (used for soft deletions / disabling log in).
        /// </summary>
        (bool success, string message) ToggleUserTypeStatus(int userTypeID, bool isActive, int userId, string ipAddress);

        /// <summary>
        /// Hard deletes a user type from the database (use with caution).
        /// </summary>
        (bool success, string message) DeleteUserType(int userTypeID, int userId, string ipAddress);

        // --- Roles ---

        /// <summary>
        /// Retrieves a complete list of all defined roles in the system.
        /// </summary>
        List<MstRoleViewModel> GetAllRoles();

        /// <summary>
        /// Retrieves a specific role's details by its ID.
        /// </summary>
        MstRoleViewModel? GetRoleByID(int roleID);

        /// <summary>
        /// Inserts or updates a Role in the system, returning the generated or updated RoleId.
        /// </summary>
        (bool success, string message, int roleId) UpsertRole(MstRoleUpsertRequest request, int userId, string ipAddress);

        /// <summary>
        /// Activates or deactivates a Role.
        /// </summary>
        (bool success, string message) ToggleRoleStatus(int roleID, bool isActive, int userId, string ipAddress);

        /// <summary>
        /// Permanently deletes a Role. Be cautious as it cascades user linkages.
        /// </summary>
        (bool success, string message) DeleteRole(int roleID, int userId, string ipAddress);

        // --- Permissions ---

        /// <summary>
        /// Retrieves the global list of all manageable permissions (modules/nodes).
        /// </summary>
        List<MstPermissionViewModel> GetAllPermissions();

        /// <summary>
        /// Retrieves a permission definition by its ID.
        /// </summary>
        MstPermissionViewModel? GetPermissionByID(int permissionID);

        /// <summary>
        /// Inserts or updates a Permission Action/Node.
        /// </summary>
        (bool success, string message) UpsertPermission(MstPermissionUpsertRequest request, int userId, string ipAddress);

        /// <summary>
        /// Toggles the usability of a specific permission node.
        /// </summary>
        (bool success, string message) TogglePermissionStatus(int permissionID, bool isActive, int userId, string ipAddress);

        /// <summary>
        /// Hard deletes a specific permission.
        /// </summary>
        (bool success, string message) DeletePermission(int permissionID, int userId, string ipAddress);

        // --- Role Permission Tree ---

        /// <summary>
        /// Retrieves the structured tree/matrix of permissions belonging to a specific role.
        /// This is primarily used to hydrate the UI checkboxes for module access.
        /// </summary>
        /// <param name="roleId">The identifier of the Role being inspected.</param>
        /// <returns>A hierarchical list of Action permissions bound to the given role.</returns>
        List<RoleMenuPermissionViewModel> GetPermissionsMatrix(int roleId);

        /// <summary>
        /// Bulk saves the permission matrix (Read/Write/Delete combinations) for a specified role.
        /// </summary>
        /// <param name="request">The mapping of role ID and the array of permission actions.</param>
        /// <param name="adminId">The ID of the admin user assigning these permissions.</param>
        /// <param name="ipAddress">The audit trace IP.</param>
        /// <returns>Execution status and database feedback message.</returns>
        (bool success, string message) SaveRolePermissions(MstRolePermissionSaveRequest request, int adminId, string ipAddress);
        /// <summary>
        /// Retrieves the aggregated list of explicit permissions granted to a user via roles or overrides.
        /// </summary>
        List<UserPermissionViewModel> GetUserPermissions(int userId);
    }
}
