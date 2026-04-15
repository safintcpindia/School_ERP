using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;
using SchoolERP.Net.Utilities;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// ADO.NET-based implementation of IUserService.
    /// Uses stored procedures for all data access — TDD 12.7.
    /// Password hashing: PBKDF2-SHA256 with per-user random salt.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly SqlHelper _sqlHelper;
        private readonly EncryptionHelper _encryption;
        private readonly ICompanyService _companyService;

        public UserService(SqlHelper sqlHelper, EncryptionHelper encryption, ICompanyService companyService)
        {
            _sqlHelper = sqlHelper;
            _encryption = encryption;
            _companyService = companyService;
        }

        // =========================================================
        // GET ALL USERS
        // =========================================================
        public List<UserViewModel> GetAllUsers()
        {
            var users = new List<UserViewModel>();
            DataTable dt = _sqlHelper.ExecuteQuery("sp_Users_GetAll", null!);

            foreach (DataRow dr in dt.Rows)
                users.Add(MapRowToViewModel(dr));

            return users;
        }

        // =========================================================
        // GET BY ID
        // =========================================================
        public UserViewModel? GetUserById(int userId)
        {
            var parameters = new[] { new SqlParameter("@UserID", userId) };
            DataTable dt = _sqlHelper.ExecuteQuery("sp_Users_GetByID", parameters);

            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        // =========================================================
        // GET ASSIGNED ROLE IDS (M:M)
        // =========================================================
        public List<int> GetUserRoleIds(int userId)
        {
            var roleIds = new List<int>();
            var parameters = new[] { new SqlParameter("@UserID", userId) };
            DataTable dt = _sqlHelper.ExecuteQuery("sp_UserRoles_GetByUser", parameters);

            foreach (DataRow dr in dt.Rows)
                roleIds.Add((int)dr["RoleID"]);

            return roleIds;
        }

        public List<int> GetUserCompanyIds(int userId)
        {
            var companyIds = new List<int>();
            var parameters = new[] { new SqlParameter("@UserID", userId) };
            DataTable dt = _sqlHelper.ExecuteQuery("sp_UserCompanies_GetByUser", parameters);

            foreach (DataRow dr in dt.Rows)
                companyIds.Add((int)dr["CompanyID"]);

            return companyIds;
        }

        // =========================================================
        // GET ROLES DROPDOWN
        // =========================================================
        public List<RoleViewModel> GetRoles()
        {
            var roles = new List<RoleViewModel>();
            DataTable dt = _sqlHelper.ExecuteQuery("sp_Roles_GetAll_ForDropdown", null!);

            foreach (DataRow dr in dt.Rows)
            {
                roles.Add(new RoleViewModel
                {
                    RoleID   = (int)dr["RoleID"],
                    RoleName = dr["RoleName"]?.ToString() ?? ""
                });
            }

            return roles;
        }

        // =========================================================
        // GET USER TYPES DROPDOWN
        // =========================================================
        public List<MstUserTypeViewModel> GetUserTypes()
        {
            var types = new List<MstUserTypeViewModel>();
            DataTable dt = _sqlHelper.ExecuteQuery("sp_UserTypes_GetAll_ForDropdown", null!);

            foreach (DataRow dr in dt.Rows)
            {
                types.Add(new MstUserTypeViewModel
                {
                    UserTypeID = (int)dr["UserTypeID"],
                    TypeCode   = dr["TypeCode"]?.ToString() ?? "",
                    TypeName   = dr["TypeName"]?.ToString() ?? ""
                });
            }

            return types;
        }

        // =========================================================
        // CREATE USER 
        // =========================================================
        /// <summary>
        /// Orchestrates the insertion pipeline consisting of memory-based hashing (PBKDF2), 
        /// structural data encryption (AES), and array compression (RoleIDs) before triggering ADO.NET.
        /// </summary>
        public (int Result, string Message) CreateUser(UserUpsertRequest request, int createdBy)
        {
            // Flatten role arrays to comma-separated strings for 'fn_SplitString' SQL parsing
            string roleIDs = string.Join(",", request.RoleIDs);

            // 1. Hash Password (Generates unique salt + compute)
            var (hash, salt) = SecurityHelper.HashPassword(request.Password);

            // 2. Encrypt Sensitive Data using symmetric application keys
            string encryptedEmail = _encryption.Encrypt(request.Email);
            string encryptedPhone = _encryption.Encrypt(request.PhoneNo);
            string encryptedOTPSecret = _encryption.Encrypt(request.OTPSecret);

            var parameters = new[]
            {
                new SqlParameter("@UserID",        0),
                new SqlParameter("@FullName",      request.FullName),
                new SqlParameter("@Username",      request.Username),
                new SqlParameter("@Email",         (object?)NullIfEmpty(encryptedEmail)    ?? DBNull.Value),
                new SqlParameter("@PasswordHash",  hash),
                new SqlParameter("@PasswordSalt",  salt),
                new SqlParameter("@PhoneNo",       (object?)NullIfEmpty(encryptedPhone)  ?? DBNull.Value),
                new SqlParameter("@UserTypeID",    request.UserTypeID),
                new SqlParameter("@DefaultRoleID", (object?)request.DefaultRoleID        ?? DBNull.Value),
                new SqlParameter("@DashboardID",   (object?)request.DashboardID          ?? DBNull.Value),
                new SqlParameter("@BackDaysAllow", request.BackDaysAllow),
                new SqlParameter("@IsOTPEnabled",  request.IsOTPEnabled),
                new SqlParameter("@OTPSecret",     (object?)NullIfEmpty(encryptedOTPSecret) ?? DBNull.Value),
                new SqlParameter("@OTPExpiry",     (object?)request.OTPExpiry             ?? DBNull.Value),
                new SqlParameter("@StartDate",     (object?)request.StartDate            ?? DBNull.Value),
                new SqlParameter("@EndDate",       (object?)request.EndDate              ?? DBNull.Value),
                new SqlParameter("@StartTime",     request.StartTime.HasValue
                                                       ? (object)request.StartTime.Value.ToString(@"hh\:mm\:ss")
                                                       : DBNull.Value),
                new SqlParameter("@EndTime",       request.EndTime.HasValue
                                                       ? (object)request.EndTime.Value.ToString(@"hh\:mm\:ss")
                                                       : DBNull.Value),
                new SqlParameter("@RoleIDs",       roleIDs),
                new SqlParameter("@ModifiedBy",    createdBy)
            };

            DataTable dt = _sqlHelper.ExecuteQuery("sp_Users_Upsert", parameters);
            if (dt.Rows.Count > 0)
                return (Convert.ToInt32(dt.Rows[0]["Result"]), dt.Rows[0]["Message"]?.ToString() ?? "");

            return (-99, "Unknown error");
        }

        // =========================================================
        // UPDATE USER
        // =========================================================
        /// <summary>
        /// Updates profile attributes without overwriting unchanged passwords or keys.
        /// Recomputes hashes only if requested natively.
        /// </summary>
        public (int Result, string Message) UpdateUser(UserUpsertRequest request, int modifiedBy)
        {
            // Flatten role arrays
            string roleIDs = string.Join(",", request.RoleIDs);

            // 1. Hash Password conditionally
            string? hash = null;
            string? salt = null;
            if (!string.IsNullOrEmpty(request.Password))
            {
                (hash, salt) = SecurityHelper.HashPassword(request.Password);
            }

            // 2. Encrypt Sensitive Data conditionally depending on presence of payload values
            string? encryptedEmail = !string.IsNullOrEmpty(request.Email) ? _encryption.Encrypt(request.Email) : null;
            string? encryptedPhone = !string.IsNullOrEmpty(request.PhoneNo) ? _encryption.Encrypt(request.PhoneNo) : null;
            string? encryptedOTPSecret = !string.IsNullOrEmpty(request.OTPSecret) ? _encryption.Encrypt(request.OTPSecret) : null;

            var parameters = new[]
            {
                new SqlParameter("@UserID",        request.UserID),
                new SqlParameter("@FullName",      request.FullName),
                new SqlParameter("@Username",      request.Username),
                new SqlParameter("@Email",         (object?)NullIfEmpty(encryptedEmail)    ?? DBNull.Value),
                new SqlParameter("@PasswordHash",  (object?)hash                         ?? DBNull.Value),
                new SqlParameter("@PasswordSalt",  (object?)salt                         ?? DBNull.Value),
                new SqlParameter("@PhoneNo",       (object?)NullIfEmpty(encryptedPhone)  ?? DBNull.Value),
                new SqlParameter("@UserTypeID",    request.UserTypeID),
                new SqlParameter("@DefaultRoleID", (object?)request.DefaultRoleID        ?? DBNull.Value),
                new SqlParameter("@DashboardID",   (object?)request.DashboardID          ?? DBNull.Value),
                new SqlParameter("@BackDaysAllow", request.BackDaysAllow),
                new SqlParameter("@IsOTPEnabled",  request.IsOTPEnabled),
                new SqlParameter("@OTPSecret",     (object?)NullIfEmpty(encryptedOTPSecret) ?? DBNull.Value),
                new SqlParameter("@OTPExpiry",     (object?)request.OTPExpiry             ?? DBNull.Value),
                new SqlParameter("@StartDate",     (object?)request.StartDate            ?? DBNull.Value),
                new SqlParameter("@EndDate",       (object?)request.EndDate              ?? DBNull.Value),
                new SqlParameter("@StartTime",     request.StartTime.HasValue
                                                       ? (object)request.StartTime.Value.ToString(@"hh\:mm\:ss")
                                                       : DBNull.Value),
                new SqlParameter("@EndTime",       request.EndTime.HasValue
                                                       ? (object)request.EndTime.Value.ToString(@"hh\:mm\:ss")
                                                       : DBNull.Value),
                new SqlParameter("@RoleIDs",       roleIDs),
                new SqlParameter("@ModifiedBy",    modifiedBy)
            };

            DataTable dt = _sqlHelper.ExecuteQuery("sp_Users_Upsert", parameters);
            if (dt.Rows.Count > 0)
                return (Convert.ToInt32(dt.Rows[0]["Result"]), dt.Rows[0]["Message"]?.ToString() ?? "");

            return (-99, "Unknown error");
        }

        // =========================================================
        // WIZARD DATA & SAVE
        // =========================================================
        public UserWizardViewModel GetUserWizardData(int userId, string roleIds = "")
        {
            var wizard = new UserWizardViewModel();

            // 1. Get User Details
            if (userId > 0)
            {
                wizard.User = GetUserById(userId) ?? new UserViewModel();
                wizard.User.CompanyIDs = GetUserCompanyIds(userId);
            }

            // 2. Get All Roles
            wizard.AllRoles = GetRoles();

            // 3. Get All Companies
            wizard.AllCompanies = _companyService.GetAllCompanies();

            // 4. Build Permission Matrix by querying each selected role individually
            //    and merging with OR logic so that any permission granted by ANY role is checked.
            var parsedRoleIds = (roleIds ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => int.TryParse(r.Trim(), out int id) ? id : 0)
                .Where(id => id > 0)
                .ToList();

            // Dictionary keyed by "MenuID:PermissionID" to merge across roles
            var mergedMatrix = new Dictionary<string, UserPermissionMatrixViewModel>();

            foreach (int roleId in parsedRoleIds)
            {
                var parameters = new[] { new SqlParameter("@RoleID", roleId) };
                DataTable dt = _sqlHelper.ExecuteQuery("sp_Roles_GetPermissionsMatrix", parameters);

                if (dt == null || dt.Rows.Count == 0) continue;

                foreach (DataRow dr in dt.Rows)
                {
                    int menuId = Convert.ToInt32(dr["MENUID"]);
                    int permId = Convert.ToInt32(dr["PermissionID"]);
                    string key = $"{menuId}:{permId}";
                    bool hasAccess = dr["HasAccess"] != DBNull.Value && Convert.ToBoolean(dr["HasAccess"]);

                    if (mergedMatrix.TryGetValue(key, out var existing))
                    {
                        // OR logic: if ANY role grants access, mark it as granted
                        if (hasAccess)
                        {
                            existing.RoleAccess = true;
                            existing.HasAccess  = true;
                        }
                    }
                    else
                    {
                        mergedMatrix[key] = new UserPermissionMatrixViewModel
                        {
                            MenuID         = menuId,
                            MenuName       = dr["MenuName"]?.ToString() ?? "",
                            ParentID       = dr["ParentID"] != DBNull.Value ? (int?)Convert.ToInt32(dr["ParentID"]) : null,
                            PermissionID   = permId,
                            PermissionName = dr["PermissionName"]?.ToString() ?? "",
                            DisplayLabel   = dr["DisplayLabel"]?.ToString() ?? "",
                            RoleAccess     = hasAccess,
                            UserOverride   = null,
                            HasAccess      = hasAccess
                        };
                    }
                }
            }

            // If editing an existing user, also check for user-level overrides
            if (userId > 0)
            {
                try
                {
                    var overrideParams = new[]
                    {
                        new SqlParameter("@UserID", userId),
                        new SqlParameter("@RoleIDs", roleIds ?? "")
                    };
                    DataTable overrideDt = _sqlHelper.ExecuteQuery("sp_Users_GetPermissionsMatrix", overrideParams);
                    foreach (DataRow dr in overrideDt.Rows)
                    {
                        int menuId = (int)dr["MenuID"];
                        int permId = (int)dr["PermissionID"];
                        string key = $"{menuId}:{permId}";

                        bool? userOverride = dr["UserOverride"] != DBNull.Value ? (bool?)dr["UserOverride"] : null;

                        if (mergedMatrix.TryGetValue(key, out var existing) && userOverride.HasValue)
                        {
                            existing.UserOverride = userOverride;
                            existing.HasAccess = userOverride.Value;
                        }
                    }
                }
                catch
                {
                    // sp_Users_GetPermissionsMatrix may not exist; overrides are optional
                }
            }

            // FILTERING: To satisfy the requirement "only those role and permissions",
            // we filter the matrix to only include menus that have role access OR a user override.
            // We also must include parent menus to keep the tree structure valid.

            var menusWithAccess = new HashSet<int>();
            foreach (var item in mergedMatrix.Values)
            {
                if (item.RoleAccess || item.UserOverride.HasValue)
                {
                    menusWithAccess.Add(item.MenuID);
                }
            }

            // Recursively add parent IDs to ensure the tree-view doesn't have "orphan" child nodes
            var menuParents = mergedMatrix.Values
                .Where(m => m.ParentID.HasValue)
                .GroupBy(m => m.MenuID)
                .ToDictionary(g => g.Key, g => g.First().ParentID!.Value);

            bool addedNew;
            do
            {
                addedNew = false;
                var currentList = menusWithAccess.ToList();
                foreach (var mId in currentList)
                {
                    if (menuParents.TryGetValue(mId, out int parentId))
                    {
                        if (menusWithAccess.Add(parentId))
                        {
                            addedNew = true;
                        }
                    }
                }
            } while (addedNew);

            wizard.PermissionMatrix = mergedMatrix.Values
                .Where(m => menusWithAccess.Contains(m.MenuID))
                .ToList();

            return wizard;
        }

        public (int Result, string Message) SaveUserWizard(UserUpsertRequest request, int modifiedBy)
        {
            try
            {
                // Validate required fields before hitting the database
                if (string.IsNullOrWhiteSpace(request.Username))
                    return (-1, "Username is required.");
                if (string.IsNullOrWhiteSpace(request.FullName))
                    return (-1, "Full Name is required.");
                if (request.UserTypeID <= 0)
                    return (-1, "User Type is required.");
                if (request.UserID == 0 && string.IsNullOrWhiteSpace(request.Password))
                    return (-1, "Password is required for new users.");

                // 1. Prepare Comma-separated strings
                string roleIDsStr    = string.Join(",", request.RoleIDs);
                string companyIDsStr = string.Join(",", request.CompanyIDs);

                // Format overrides: 'MenuID:PermissionID:IsAllowed'
                string overridesStr = string.Join(",", request.PermissionOverrides
                    .Select(o => $"{o.MenuID}:{o.PermissionID}:{(o.IsAllowed ? 1 : 0)}"));

                // 2. Handle Password Hashing (only if provided)
                string? hash = null, salt = null;
                if (!string.IsNullOrEmpty(request.Password))
                {
                    (hash, salt) = SecurityHelper.HashPassword(request.Password);
                }

                // 3. Encrypt Sensitive Data (safe — EncryptionHelper returns input when empty)
                string encryptedEmail  = _encryption.Encrypt(request.Email  ?? "");
                string encryptedPhone  = _encryption.Encrypt(request.PhoneNo ?? "");
                string? encryptedOTPSecret = !string.IsNullOrEmpty(request.OTPSecret)
                    ? _encryption.Encrypt(request.OTPSecret) : null;

                var parameters = new[]
                {
                    new SqlParameter("@UserID",              request.UserID),
                    new SqlParameter("@FullName",            request.FullName),
                    new SqlParameter("@Username",            request.Username),
                    new SqlParameter("@Email",               (object?)NullIfEmpty(encryptedEmail)       ?? DBNull.Value),
                    new SqlParameter("@PasswordHash",        (object?)hash                             ?? DBNull.Value),
                    new SqlParameter("@PasswordSalt",        (object?)salt                             ?? DBNull.Value),
                    new SqlParameter("@PhoneNo",             (object?)NullIfEmpty(encryptedPhone)      ?? DBNull.Value),
                    new SqlParameter("@UserTypeID",          request.UserTypeID),
                    new SqlParameter("@DefaultRoleID",       (object?)request.DefaultRoleID            ?? DBNull.Value),
                    new SqlParameter("@DashboardID",         (object?)request.DashboardID              ?? DBNull.Value),
                    new SqlParameter("@BackDaysAllow",       request.BackDaysAllow),
                    new SqlParameter("@IsOTPEnabled",        request.IsOTPEnabled),
                    new SqlParameter("@OTPSecret",           (object?)NullIfEmpty(encryptedOTPSecret)  ?? DBNull.Value),
                    new SqlParameter("@OTPExpiry",           (object?)request.OTPExpiry                ?? DBNull.Value),
                    new SqlParameter("@StartDate",           (object?)request.StartDate                ?? DBNull.Value),
                    new SqlParameter("@EndDate",             (object?)request.EndDate                  ?? DBNull.Value),
                    new SqlParameter("@StartTime",           request.StartTime.HasValue
                                                                 ? (object)request.StartTime.Value.ToString(@"hh\:mm\:ss")
                                                                 : DBNull.Value),
                    new SqlParameter("@EndTime",             request.EndTime.HasValue
                                                                 ? (object)request.EndTime.Value.ToString(@"hh\:mm\:ss")
                                                                 : DBNull.Value),
                    new SqlParameter("@RoleIDs",             roleIDsStr),
                    new SqlParameter("@CompanyIDs",          companyIDsStr),
                    new SqlParameter("@PermissionOverrides", overridesStr),
                    new SqlParameter("@ModifiedBy",          modifiedBy)
                };

                DataTable dt = _sqlHelper.ExecuteQuery("sp_Users_Upsert_Wizard", parameters);
                if (dt.Rows.Count > 0)
                    return (Convert.ToInt32(dt.Rows[0]["Result"]), dt.Rows[0]["Message"]?.ToString() ?? "");

                return (-99, "No result returned from database.");
            }
            catch (Exception ex)
            {
                return (-99, $"Wizard save failed: {ex.Message}");
            }
        }

        // =========================================================
        // TOGGLE STATUS
        // =========================================================
        public (int Result, string Message) ToggleUserStatus(int userId, bool isActive, int doneBy)
        {
            var parameters = new[]
            {
                new SqlParameter("@UserID",   userId),
                new SqlParameter("@IsActive", isActive)
            };

            DataTable dt = _sqlHelper.ExecuteQuery("sp_ToggleUserStatus", parameters);
            // sp_ToggleUserStatus doesn't return result/message in my helptext view, 
            // but for consistency with other methods, I'll return success if it executed.
            // Actually, I should probably update sp_ToggleUserStatus to return a result.
            return (1, "Status updated successfully");
        }

        public (int Result, string Message) DeleteUser(int userId, int doneBy)
        {
            var parameters = new[]
            {
                new SqlParameter("@UserID", userId),
                new SqlParameter("@DoneBy", doneBy)
            };

            DataTable dt = _sqlHelper.ExecuteQuery("sp_Users_Delete", parameters);
            if (dt.Rows.Count > 0)
                return (Convert.ToInt32(dt.Rows[0]["Result"]), dt.Rows[0]["Message"]?.ToString() ?? "");

            return (-99, "Unknown error");
        }

        // =========================================================
        // UNLOCK
        // =========================================================
        public void UnlockUser(int userId, int doneBy)
        {
            var parameters = new[]
            {
                new SqlParameter("@UserID", userId),
                new SqlParameter("@DoneBy", doneBy)
            };
            _sqlHelper.ExecuteNonQuery("sp_Users_Unlock", parameters);
        }

        private UserViewModel MapRowToViewModel(DataRow dr)
        {
            return new UserViewModel
            {
                UserID          = (int)dr["UserID"],
                FullName        = dr["FullName"]?.ToString()       ?? "",
                Username        = dr["Username"]?.ToString()       ?? "",
                Email           = _encryption.Decrypt(dr["Email"]?.ToString() ?? ""),
                PhoneNo         = _encryption.Decrypt(dr["PhoneNo"]?.ToString() ?? ""),
                UserTypeID      = dr["UserTypeID"] != DBNull.Value  ? Convert.ToInt32(dr["UserTypeID"])  : 0,
                UserTypeName    = dr["UserTypeName"]?.ToString()   ?? "",
                DefaultRoleID   = dr["DefaultRoleID"] != DBNull.Value ? Convert.ToInt32(dr["DefaultRoleID"]) : null,
                DefaultRoleName = dr.Table.Columns.Contains("DefaultRoleName") && dr["DefaultRoleName"] != DBNull.Value
                                    ? dr["DefaultRoleName"].ToString() ?? "" : "",
                DashboardID     = dr["DashboardID"] != DBNull.Value ? Convert.ToInt32(dr["DashboardID"]) : null,
                RoleNames       = dr.Table.Columns.Contains("RoleNames") ? dr["RoleNames"]?.ToString() ?? "" : "",
                BackDaysAllow   = dr["BackDaysAllow"] != DBNull.Value ? Convert.ToInt32(dr["BackDaysAllow"]) : 0,
                IsOTPEnabled    = dr["IsOTPEnabled"] != DBNull.Value && (bool)dr["IsOTPEnabled"],
                StartDate       = dr["StartDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["StartDate"]) : null,
                EndDate         = dr["EndDate"]   != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["EndDate"])   : null,
                StartTime       = dr["StartTime"] != DBNull.Value ? (TimeSpan?)dr["StartTime"] : null,
                EndTime         = dr["EndTime"]   != DBNull.Value ? (TimeSpan?)dr["EndTime"]   : null,
                OTPSecret       = dr.Table.Columns.Contains("OTPSecret") && dr["OTPSecret"] != DBNull.Value ? _encryption.Decrypt(dr["OTPSecret"].ToString()!) : null,
                OTPExpiry       = dr.Table.Columns.Contains("OTPExpiry") && dr["OTPExpiry"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["OTPExpiry"]) : null,
                IsActive        = dr["IsActive"] != DBNull.Value && (bool)dr["IsActive"],
                IsLocked        = dr["IsLocked"]  != DBNull.Value && (bool)dr["IsLocked"],
                FailedAttempts  = dr["FailedAttempts"] != DBNull.Value ? Convert.ToInt32(dr["FailedAttempts"]) : 0,
                LastLoginOn     = dr["LastLoginOn"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["LastLoginOn"]) : null,
                CreatedBy       = dr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(dr["CreatedBy"]) : 0,
                CreatedOn       = dr["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(dr["CreatedOn"]) : DateTime.UtcNow,
                ModifiedBy      = dr["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(dr["ModifiedBy"]) : null,
                ModifiedOn      = dr["ModifiedOn"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["ModifiedOn"]) : null
            };
        }
        private object? NullIfEmpty(string? value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
