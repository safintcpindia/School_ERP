using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// Service interface for User Management operations.
    /// All data access uses ADO.NET stored procedures (no ORM).
    /// Aligned with TDD 12.7 tbl_mst_users schema.
    /// </summary>
    public interface IUserService
    {
        /// <summary>Gets all users with type/role info.</summary>
        List<UserViewModel> GetAllUsers();

        /// <summary>Gets a single user by UserID (includes role assignments).</summary>
        UserViewModel? GetUserById(int userId);

        /// <summary>Gets the list of RoleIDs assigned to a user (M:M).</summary>
        List<int> GetUserRoleIds(int userId);

        /// <summary>Gets all active roles for dropdown.</summary>
        List<RoleViewModel> GetRoles();

        /// <summary>Gets all active user types for dropdown.</summary>
        List<MstUserTypeViewModel> GetUserTypes();

        /// <summary>Creates a new user with hashed password + role mappings.</summary>
        (int Result, string Message) CreateUser(UserUpsertRequest request, int createdBy);

        /// <summary>Updates an existing user and re-assigns roles.</summary>
        (int Result, string Message) UpdateUser(UserUpsertRequest request, int modifiedBy);

        /// <summary>Gets the list of CompanyIDs assigned to a user (M:M).</summary>
        List<int> GetUserCompanyIds(int userId);

        /// <summary>
        /// Gets all data required to hydrate the 3-step User Wizard.
        /// Includes user info, roles, companies, and combined permission matrix.
        /// </summary>
        UserWizardViewModel GetUserWizardData(int userId, string roleIds = "");

        /// <summary>
        /// Saves all 3 steps of the user wizard in a single transactional operation.
        /// Handles identity, company mappings, and permission overrides.
        /// </summary>
        (int Result, string Message) SaveUserWizard(UserUpsertRequest request, int modifiedBy);

        /// <summary>Toggles user active status (soft delete/restore).</summary>
        (int Result, string Message) ToggleUserStatus(int userId, bool isActive, int doneBy);

        /// <summary>Soft deletes a user.</summary>
        (int Result, string Message) DeleteUser(int userId, int doneBy);

        /// <summary>Unlocks a locked user account.</summary>
        void UnlockUser(int userId, int doneBy);

        /// <summary>Checks if a username is structurally unique among active users.</summary>
        bool IsUsernameUnique(string username, int userId);
    }
}
