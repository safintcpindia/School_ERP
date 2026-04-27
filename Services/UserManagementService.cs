using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This service handles the actual work of managing user categories, roles, and permissions, ensuring the right people have access to the right parts of the system.
    /// </summary>
    public class UserManagementService : IUserManagementService
    {
        private readonly SqlHelper _sqlHelper;

        public UserManagementService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        // --- User Types ---
        
        /// <summary>
        /// Retrieves a list of all user categories from the database.
        /// </summary>
        public List<MstUserTypeViewModel> GetAllUserTypes()
        {
            var list = new List<MstUserTypeViewModel>();
            var dt = _sqlHelper.ExecuteQuery("sp_UserTypes_GetAll", null!);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new MstUserTypeViewModel
                {
                    UserTypeID = Convert.ToInt32(row["UserTypeID"]),
                    TypeCode = row["TypeCode"].ToString() ?? "",
                    TypeName = row["TypeName"].ToString() ?? "",
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedOn = Convert.ToDateTime(row["CreatedOn"])
                });
            }
            return list;
        }

        /// <summary>
        /// Looks up a specific user category's details from the database.
        /// </summary>
        public MstUserTypeViewModel? GetUserTypeByID(int userTypeID)
        {
            var dt = _sqlHelper.ExecuteQuery("sp_UserTypes_GetByID", new[] { new SqlParameter("@UserTypeID", userTypeID) });
            if (dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            return new MstUserTypeViewModel
            {
                UserTypeID = Convert.ToInt32(row["UserTypeID"]),
                TypeCode = row["TypeCode"].ToString() ?? "",
                TypeName = row["TypeName"].ToString() ?? "",
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };
        }

        /// <summary>
        /// Saves or updates a user category in the database.
        /// </summary>
        public (bool success, string message) UpsertUserType(MstUserTypeUpsertRequest request, int userId, string ipAddress)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@UserTypeID", request.UserTypeID),
                    new SqlParameter("@TypeCode", request.TypeCode),
                    new SqlParameter("@TypeName", request.TypeName),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@IPAddress", ipAddress)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_UserTypes_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Updates whether a user category is active or not.
        /// </summary>
        public (bool success, string message) ToggleUserTypeStatus(int userTypeID, bool isActive, int userId, string ipAddress)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@UserTypeID", userTypeID),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@IPAddress", ipAddress)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_UserTypes_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a user category from the database.
        /// </summary>
        public (bool success, string message) DeleteUserType(int userTypeID, int userId, string ipAddress)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@UserTypeID", userTypeID),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@IPAddress", ipAddress)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_UserTypes_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        // --- Roles ---
        
        /// <summary>
        /// Retrieves a list of all roles from the database.
        /// </summary>
        public List<MstRoleViewModel> GetAllRoles()
        {
            var list = new List<MstRoleViewModel>();
            var dt = _sqlHelper.ExecuteQuery("sp_Roles_GetAll", null!);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new MstRoleViewModel
                {
                    RoleID = Convert.ToInt32(row["RoleID"]),
                    RoleName = row["RoleName"].ToString() ?? "",
                    RoleDesc = row["RoleDesc"]?.ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedOn = Convert.ToDateTime(row["CreatedOn"])
                });
            }
            return list;
        }

        /// <summary>
        /// Looks up a specific role's details from the database.
        /// </summary>
        public MstRoleViewModel? GetRoleByID(int roleID)
        {
            var dt = _sqlHelper.ExecuteQuery("sp_Roles_GetByID", new[] { new SqlParameter("@RoleID", roleID) });
            if (dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            return new MstRoleViewModel
            {
                RoleID = Convert.ToInt32(row["RoleID"]),
                RoleName = row["RoleName"].ToString() ?? "",
                RoleDesc = row["RoleDesc"]?.ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };
        }

        /// <summary>
        /// Saves or updates a role in the database.
        /// </summary>
        public (bool success, string message, int roleId) UpsertRole(MstRoleUpsertRequest request, int userId, string ipAddress)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@RoleID", request.RoleID),
                    new SqlParameter("@RoleName", request.RoleName),
                    new SqlParameter("@RoleDesc", (object?)request.RoleDesc ?? DBNull.Value),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@IPAddress", ipAddress)
                };
                
                // Safely fire ADO.NET connection wrapper
                var dt = _sqlHelper.ExecuteQuery("sp_Roles_Upsert", parameters);
                
                if (dt == null || dt.Rows.Count == 0)
                    return (false, "No responsive mapping detected from database trace", 0);

                bool ok = Convert.ToInt32(dt.Rows[0]["Result"]) == 1;
                string msg = dt.Rows[0]["Message"]?.ToString() ?? "";
                
                // Authoritatively parse SCOPE_IDENTITY() output from the Stored Procedure if generated
                int savedRoleId = dt.Columns.Contains("RoleID") && dt.Rows[0]["RoleID"] != DBNull.Value
                    ? Convert.ToInt32(dt.Rows[0]["RoleID"])
                    : (request.RoleID > 0 ? request.RoleID : 0);

                return (ok, msg, savedRoleId);
            }
            catch (Exception ex) { return (false, ex.Message, 0); }
        }

        /// <summary>
        /// Updates whether a role is active or not.
        /// </summary>
        public (bool success, string message) ToggleRoleStatus(int roleID, bool isActive, int userId, string ipAddress)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@RoleID", roleID),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@IPAddress", ipAddress)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Roles_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a role from the database.
        /// </summary>
        public (bool success, string message) DeleteRole(int roleID, int userId, string ipAddress)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@RoleID", roleID),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@IPAddress", ipAddress)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Roles_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        // --- Permissions ---
        
        /// <summary>
        /// Retrieves a list of all possible actions/permissions from the database.
        /// </summary>
        public List<MstPermissionViewModel> GetAllPermissions()
        {
            var list = new List<MstPermissionViewModel>();
            var dt = _sqlHelper.ExecuteQuery("sp_Permissions_GetAll", null!);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new MstPermissionViewModel
                {
                    PermissionID = Convert.ToInt32(row["PermissionID"]),
                    PermissionName = row["PermissionName"].ToString() ?? "",
                    DisplayLabel = row["DisplayLabel"]?.ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedOn = Convert.ToDateTime(row["CreatedOn"])
                });
            }
            return list;
        }

        /// <summary>
        /// Looks up a specific permission's details from the database.
        /// </summary>
        public MstPermissionViewModel? GetPermissionByID(int permissionID)
        {
            var dt = _sqlHelper.ExecuteQuery("sp_Permissions_GetByID", new[] { new SqlParameter("@PermissionID", permissionID) });
            if (dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            return new MstPermissionViewModel
            {
                PermissionID = Convert.ToInt32(row["PermissionID"]),
                PermissionName = row["PermissionName"].ToString() ?? "",
                DisplayLabel = row["DisplayLabel"]?.ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };
        }

        /// <summary>
        /// Saves or updates a permission in the database.
        /// </summary>
        public (bool success, string message) UpsertPermission(MstPermissionUpsertRequest request, int userId, string ipAddress)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@PermissionID", request.PermissionID),
                    new SqlParameter("@PermissionName", request.PermissionName),
                    new SqlParameter("@DisplayLabel", (object?)request.DisplayLabel ?? DBNull.Value),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@IPAddress", ipAddress)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Permissions_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Updates whether a permission is currently usable.
        /// </summary>
        public (bool success, string message) TogglePermissionStatus(int permissionID, bool isActive, int userId, string ipAddress)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@PermissionID", permissionID),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@IPAddress", ipAddress)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Permissions_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a permission from the database.
        /// </summary>
        public (bool success, string message) DeletePermission(int permissionID, int userId, string ipAddress)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@PermissionID", permissionID),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@IPAddress", ipAddress)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Permissions_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        // --- Role Permission Tree ---
        
        /// <summary>
        /// Fetches the full list of allowed actions for a role from the database.
        /// </summary>
        public List<RoleMenuPermissionViewModel> GetPermissionsMatrix(int roleId)
        {
            var list = new List<RoleMenuPermissionViewModel>();
            var parameters = new[] { new SqlParameter("@RoleID", roleId) };
            var dt = _sqlHelper.ExecuteQuery("sp_Roles_GetPermissionsMatrix", parameters);
            
            if (dt == null || dt.Rows.Count == 0) return list;
            if (!HasMatrixColumns(dt))
                return list;

            string? menuIdColumn = FindColumnName(dt, "MENUID", "MenuID", "MenuId","MenuIcon");
            if (string.IsNullOrWhiteSpace(menuIdColumn))
                throw new ArgumentException("Column 'MenuID' does not belong to table.");

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RoleMenuPermissionViewModel
                {
                    MenuID = Convert.ToInt32(row[menuIdColumn]),
                    MenuName = row["MenuName"]?.ToString() ?? "",
                    MenuIcon = row["MenuIcon"]?.ToString() ?? "",
                    ParentID = row["ParentID"] != DBNull.Value ? Convert.ToInt32(row["ParentID"]) : null,
                    PermissionID = Convert.ToInt32(row["PermissionID"]),
                    PermissionName = row["PermissionName"]?.ToString() ?? "",
                    DisplayLabel = row["DisplayLabel"]?.ToString() ?? "",
                    HasAccess = row["HasAccess"] != DBNull.Value && Convert.ToBoolean(row["HasAccess"])
                });
            }
            return list;
        }

        /// <summary>
        /// Saves the mapping of actions to roles in the database.
        /// </summary>
        public (bool success, string message) SaveRolePermissions(MstRolePermissionSaveRequest request, int adminId, string ipAddress)
        {
            try
            {
                string pairs = string.Join(",", request.SelectedPermissions.Select(p => $"{p.MenuID}:{p.PermissionID}"));
                var parameters = new[]
                {
                    new SqlParameter("@RoleID", request.RoleID),
                    new SqlParameter("@MenuPermissionPairs", pairs),
                    new SqlParameter("@UserId", adminId),
                    new SqlParameter("@IPAddress", ipAddress)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Roles_SavePermissions", parameters);
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"]?.ToString() ?? "");
                }
                return (false, "No response from database");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Retrieves the combined list of all actions a specific user is permitted to perform.
        /// </summary>
        public List<UserPermissionViewModel> GetUserPermissions(int userId)
        {
            var list = new List<UserPermissionViewModel>();
            var dt = _sqlHelper.ExecuteQuery("sp_User_GetPermissions", new[] { new SqlParameter("@UserID", userId) });
            if (dt == null || dt.Rows.Count == 0) return list;

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new UserPermissionViewModel
                {
                    MenuID = Convert.ToInt32(row["MenuID"]),
                    PermissionName = row["PermissionName"]?.ToString() ?? "",
                    MenuURL = row["MenuURL"]?.ToString() ?? "",
                    MenuKey = row["MenuKey"]?.ToString() ?? ""
                });
            }
            return list;
        }

        private static string? FindColumnName(DataTable table, params string[] expectedNames)
        {
            foreach (DataColumn col in table.Columns)
            {
                foreach (string expected in expectedNames)
                {
                    if (string.Equals(col.ColumnName, expected, StringComparison.OrdinalIgnoreCase))
                        return col.ColumnName;
                }
            }
            return null;
        }

        private static bool HasMatrixColumns(DataTable table)
        {
            return !string.IsNullOrWhiteSpace(FindColumnName(table, "MENUID", "MenuID", "MenuId"))
                   && !string.IsNullOrWhiteSpace(FindColumnName(table, "PermissionID", "PERMISSIONID", "PermissionId"));
        }
    }
}
