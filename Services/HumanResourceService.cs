using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This service handles the actual work of managing human resources, such as saving designations, departments, leave types, and staff records in the database.
    /// </summary>
    public class HumanResourceService : IHumanResourceService
    {
        private readonly SqlHelper _db;
        private readonly IUserService _userService;
        public HumanResourceService(SqlHelper db, IUserService userService)
        {
            _db = db;
            _userService = userService;
        }

        // --- Designation ---
        /// <summary>
        /// Retrieves a complete list of all job designations for the current school and session from the database.
        /// </summary>
        public List<HRDesignationViewModel> GetAllDesignations(int companyId, int sessionId)
        {
            var list = new List<HRDesignationViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_Mst_HRDesignation_GetAll", p).Rows)
                    list.Add(MapDesignation(row));
            }
            catch (Exception) { }
            return list;
        }

        /// <summary>
        /// Looks up the details of a specific designation using its unique ID.
        /// </summary>
        public HRDesignationViewModel? GetDesignationByID(int id)
        {
            var p = new[] { new SqlParameter("@HRDesignationID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_HRDesignation_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapDesignation(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates a designation record in the database.
        /// </summary>
        public (bool Success, string Message) UpsertDesignation(HRDesignationUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@HRDesignationID", req.HRDesignationID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@DesignationName", req.DesignationName),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_HRDesignation_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a designation's record from the database.
        /// </summary>
        public (bool Success, string Message) DeleteDesignation(int id, int userId)
        {
            try
            {
                var p = new[] { 
                    new SqlParameter("@HRDesignationID", id), 
                    new SqlParameter("@UserID", userId) 
                };
                var dt = _db.ExecuteQuery("sp_Mst_HRDesignation_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Updates whether a designation is currently active or inactive.
        /// </summary>
        public (bool Success, string Message) ToggleDesignationStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@HRDesignationID", id),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_HRDesignation_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        // --- Department ---
        /// <summary>
        /// Retrieves a complete list of all departments for the current school and session from the database.
        /// </summary>
        public List<HRDepartmentViewModel> GetAllDepartments(int companyId, int sessionId)
        {
            var list = new List<HRDepartmentViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_Mst_Department_GetAll", p).Rows)
                    list.Add(MapDepartment(row));
            }
            catch (Exception) { }
            return list;
        }

        /// <summary>
        /// Looks up the details of a specific department using its unique ID.
        /// </summary>
        public HRDepartmentViewModel? GetDepartmentByID(int id)
        {
            var p = new[] { new SqlParameter("@DepartmentID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_Department_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapDepartment(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates a department record in the database.
        /// </summary>
        public (bool Success, string Message) UpsertDepartment(HRDepartmentUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@DepartmentID", req.DepartmentID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@DepartmentName", req.DepartmentName),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_Department_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a department's record from the database.
        /// </summary>
        public (bool Success, string Message) DeleteDepartment(int id, int userId)
        {
            try
            {
                var p = new[] { 
                    new SqlParameter("@DepartmentID", id), 
                    new SqlParameter("@UserID", userId) 
                };
                var dt = _db.ExecuteQuery("sp_Mst_Department_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Updates whether a department is currently active or inactive.
        /// </summary>
        public (bool Success, string Message) ToggleDepartmentStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@DepartmentID", id),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_Department_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        // --- Leave Type ---
        /// <summary>
        /// Retrieves a complete list of all leave types for the current school and session from the database.
        /// </summary>
        public List<HRLeaveTypeViewModel> GetAllLeaveTypes(int companyId, int sessionId)
        {
            var list = new List<HRLeaveTypeViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_Mst_LeaveType_GetAll", p).Rows)
                    list.Add(MapLeaveType(row));
            }
            catch (Exception) { }
            return list;
        }

        /// <summary>
        /// Looks up the details of a specific leave type using its unique ID.
        /// </summary>
        public HRLeaveTypeViewModel? GetLeaveTypeByID(int id)
        {
            var p = new[] { new SqlParameter("@LeaveTypeID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_LeaveType_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapLeaveType(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates a leave type record in the database.
        /// </summary>
        public (bool Success, string Message) UpsertLeaveType(HRLeaveTypeUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@LeaveTypeID", req.LeaveTypeID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@LeaveTypeName", req.LeaveTypeName),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_LeaveType_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a leave type's record from the database.
        /// </summary>
        public (bool Success, string Message) DeleteLeaveType(int id, int userId)
        {
            try
            {
                var p = new[] { 
                    new SqlParameter("@LeaveTypeID", id),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_LeaveType_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public dynamic GetLeaveBalance(int staffId, int leaveTypeId, int companyId, int sessionId)
        {
            try
            {
                // Fallback: If companyId/sessionId is 0, try to get them from the staff record
                if (companyId <= 0 || sessionId <= 0)
                {
                    var staff = GetStaffByID(staffId);
                    if (staff != null)
                    {
                        if (companyId <= 0) companyId = staff.CompanyID;
                        if (sessionId <= 0) sessionId = staff.SessionID;
                    }
                }

                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@StaffID", staffId),
                    new SqlParameter("@LeaveTypeID", leaveTypeId)
                };
                var dt = _db.ExecuteQuery("sp_HR_Leave_GetBalance", p);
                if (dt.Rows.Count > 0)
                {
                    return new
                    {
                        TotalQuota = Convert.ToDecimal(dt.Rows[0]["TotalQuota"]),
                        TotalUsed = Convert.ToDecimal(dt.Rows[0]["TotalUsed"]),
                        Balance = Convert.ToDecimal(dt.Rows[0]["Balance"]),
                        Message = dt.Rows[0]["Message"].ToString()
                    };
                }
            }
            catch { }
            return new { TotalQuota = 0, TotalUsed = 0, Balance = 0, Message = "Unable to fetch balance" };
        }

        public List<dynamic> GetStaffAllLeaveBalances(int staffId, int companyId, int sessionId)
        {
            var result = new List<dynamic>();
            try
            {
                var p = new[] { new SqlParameter("@StaffID", staffId) };
                var dt = _db.ExecuteQuery("sp_HR_Leave_GetStaffBalances", p);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        result.Add(new
                        {
                            leaveTypeID = Convert.ToInt32(r["LeaveTypeID"]),
                            leaveTypeName = r["LeaveTypeName"].ToString(),
                            totalQuota = Convert.ToDecimal(r["TotalQuota"]),
                            totalUsed = Convert.ToDecimal(r["TotalUsed"]),
                            balance = Convert.ToDecimal(r["Balance"])
                        });
                    }
                }
                else
                {
                    // Fallback logic
                    if (companyId <= 0 || sessionId <= 0)
                    {
                        var staff = GetStaffByID(staffId);
                        if (staff != null)
                        {
                            companyId = staff.CompanyID;
                            sessionId = staff.SessionID;
                        }
                    }

                    var leaveTypes = GetAllLeaveTypes(companyId, sessionId);
                    foreach (var lt in leaveTypes)
                    {
                        var bal = GetLeaveBalance(staffId, lt.LeaveTypeID, companyId, sessionId);
                        
                        // If bal is 0, try to get just the quota as a last resort
                        decimal q = (decimal)bal.TotalQuota;
                        if (q == 0) q = GetStaffLeaveQuota(companyId, sessionId, staffId, lt.LeaveTypeID);

                        result.Add(new
                        {
                            leaveTypeID = lt.LeaveTypeID,
                            leaveTypeName = lt.LeaveTypeName,
                            totalQuota = q,
                            totalUsed = (decimal)bal.TotalUsed,
                            balance = q - (decimal)bal.TotalUsed
                        });
                    }
                }
            }
            catch (Exception)
            {
                try
                {
                    var leaveTypes = GetAllLeaveTypes(companyId, sessionId);
                    foreach (var lt in leaveTypes)
                        result.Add(new { leaveTypeID = lt.LeaveTypeID, leaveTypeName = lt.LeaveTypeName, totalQuota = 0m, totalUsed = 0m, balance = 0m });
                }
                catch { }
            }
            return result;
        }


        /// <summary>
        /// Updates whether a leave type is currently active or inactive.
        /// </summary>
        public (bool Success, string Message) ToggleLeaveTypeStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@LeaveTypeID", id),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_LeaveType_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        // --- Staff ---
        /// <summary>
        /// Retrieves a complete list of all staff members for the current school and session from the database.
        /// </summary>
        public List<HRStaffViewModel> GetAllStaff(int companyId, int sessionId)
        {
            var list = new List<HRStaffViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@IncludeDeleted", false)
                };
                var dt = _db.ExecuteQuery("SP_HR_STAFF_GETALL", p);
                
                if (dt.Rows.Count > 0)
                {
                    // Check if the SP returned an error/empty message set (Result=0)
                    if (dt.Columns.Contains("RESULT") && Convert.ToInt32(dt.Rows[0]["RESULT"]) == 0)
                    {
                        return list; // Return empty list as per SP validation
                    }

                    // Optimization: Fetch all roles once to map names efficiently
                    var allRoles = _userService.GetRoles();
                    var roleMap = allRoles.ToDictionary(r => r.RoleID, r => r.RoleName);

                    foreach (DataRow row in dt.Rows)
                    {
                        var staff = MapStaff(row);
                        
                        // Populate DisplayRoles using UserService as requested
                        if (staff.UserID.HasValue && staff.UserID > 0)
                        {
                            try
                            {
                                var userRoleIds = _userService.GetUserRoleIds(staff.UserID.Value);
                                foreach (var rid in userRoleIds)
                                {
                                    if (roleMap.TryGetValue(rid, out var rName))
                                        staff.DisplayRoles.Add(rName);
                                }
                            }
                            catch { /* Skip roles if fetch fails */ }
                        }
                        
                        list.Add(staff);
                    }
                }
            }
            catch (Exception) { }
            return list;
        }

        /// <summary>
        /// Looks up the details of a specific staff member using its unique ID, including their roles and allowed school branches.
        /// </summary>
        public HRStaffViewModel? GetStaffByID(int id)
        {
            var p = new[] { new SqlParameter("@StaffID", id) };
            var ds = _db.ExecuteDataSet("sp_HR_Staff_GetByID", p);
            
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;

            var staff = MapStaff(ds.Tables[0].Rows[0]);
            
            // Roles (Second Table)
            if (ds.Tables.Count > 1)
            {
                foreach (DataRow r in ds.Tables[1].Rows)
                    staff.RoleIDs.Add(Convert.ToInt32(r["RoleID"]));
            }

            // Companies (Third Table)
            if (ds.Tables.Count > 2)
            {
                foreach (DataRow r in ds.Tables[2].Rows)
                {
                    int cId = r.Table.Columns.Contains("CompanyID") ? Convert.ToInt32(r["CompanyID"]) : 
                              (r.Table.Columns.Contains("CompanyId") ? Convert.ToInt32(r["CompanyId"]) : 0);
                    if (cId > 0 && !staff.CompanyIDs.Contains(cId))
                        staff.CompanyIDs.Add(cId);
                }
            }

            // Leave Quotas (Fourth Table)
            if (ds.Tables.Count > 3)
            {
                foreach (DataRow r in ds.Tables[3].Rows)
                {
                    staff.LeaveQuotas.Add(new HRStaffLeaveQuotaViewModel
                    {
                        LeaveTypeID = Convert.ToInt32(r["LeaveTypeID"]),
                        LeaveTypeName = r["LeaveTypeName"].ToString()!,
                        MaxDays = Convert.ToDecimal(r["MaxDays"])
                    });
                }
            }

            // Fallback: Ensure the primary CompanyID is also in the list
            if (staff.CompanyID > 0 && !staff.CompanyIDs.Contains(staff.CompanyID))
                staff.CompanyIDs.Add(staff.CompanyID);

            // NEW: Fetch roles and companies assigned to the linked User account (Source of Truth)
            if (staff.UserID.HasValue && staff.UserID > 0)
            {
                try
                {
                    // Sync Roles
                    var userRolesDt = _db.ExecuteQuery("sp_UserRoles_GetByUser", new[] { new SqlParameter("@UserID", staff.UserID.Value) });
                    foreach (DataRow r in userRolesDt.Rows)
                    {
                        int rId = Convert.ToInt32(r["RoleID"]);
                        if (rId > 0 && !staff.RoleIDs.Contains(rId))
                            staff.RoleIDs.Add(rId);
                    }

                    // Sync Companies
                    var userCompDt = _db.ExecuteQuery("sp_UserCompanies_GetByUser", new[] { new SqlParameter("@UserID", staff.UserID.Value) });
                    foreach (DataRow r in userCompDt.Rows)
                    {
                        int cId = r.Table.Columns.Contains("CompanyID") ? Convert.ToInt32(r["CompanyID"]) : 
                                  (r.Table.Columns.Contains("CompanyId") ? Convert.ToInt32(r["CompanyId"]) : 0);
                        if (cId > 0 && !staff.CompanyIDs.Contains(cId))
                            staff.CompanyIDs.Add(cId);
                    }
                    // Sync DisplayRoles (Names)
                    var allRoles = _userService.GetRoles();
                    var roleMap = allRoles.ToDictionary(r => r.RoleID, r => r.RoleName);
                    foreach (var rid in staff.RoleIDs)
                    {
                        if (roleMap.TryGetValue(rid, out var rName) && !staff.DisplayRoles.Contains(rName))
                            staff.DisplayRoles.Add(rName);
                    }

                    // Dynamic Leave Quotas are already populated from Table 3 in the DataSet above
                }
                catch (Exception) { /* Handle or log if needed */ }
            }

            return staff;
        }

        private decimal GetStaffLeaveQuota(int companyId, int sessionId, int staffId, int leaveTypeId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@StaffID", staffId),
                    new SqlParameter("@LeaveTypeID", leaveTypeId)
                };
                var dt = _db.ExecuteQuery("sp_HR_StaffLeaveQuota_Get", p);
                if (dt.Rows.Count > 0)
                    return Convert.ToDecimal(dt.Rows[0]["MaxDays"]);
            }
            catch { }
            return 0;
        }

        /// <summary>
        /// Saves or updates a staff member's record in the database, including personal info, documents, and system permissions.
        /// </summary>
        public (bool Success, string Message) UpsertStaff(HRStaffUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                // If companyId/sessionId is 0 (e.g. from global context), try to resolve from staff record for updates
                if (req.StaffID > 0 && (companyId <= 0 || sessionId <= 0))
                {
                    var staff = GetStaffByID(req.StaffID);
                    if (staff != null)
                    {
                        if (companyId <= 0) companyId = staff.CompanyID;
                        if (sessionId <= 0) sessionId = staff.SessionID;
                    }
                }

                // Auto-generate Password for new staff if not provided
                if (req.StaffID == 0 && string.IsNullOrEmpty(req.PasswordPlain))
                {
                    req.PasswordPlain = GenerateRandomPassword();
                }

                // Auto-generate StaffCode if not provided
                if (string.IsNullOrEmpty(req.StaffCode))
                {
                    req.StaffCode = GetNewStaffCode(companyId, sessionId);
                }

                var photoBytes = string.IsNullOrEmpty(req.PhotoBase64) ? null : Convert.FromBase64String(req.PhotoBase64.Split(',').Last());
                var resumeBytes = string.IsNullOrEmpty(req.ResumeBase64) ? null : Convert.FromBase64String(req.ResumeBase64.Split(',').Last());
                var joiningBytes = string.IsNullOrEmpty(req.JoiningLetterBase64) ? null : Convert.FromBase64String(req.JoiningLetterBase64.Split(',').Last());
                var resignBytes = string.IsNullOrEmpty(req.ResignationLetterBase64) ? null : Convert.FromBase64String(req.ResignationLetterBase64.Split(',').Last());
                var otherBytes = string.IsNullOrEmpty(req.OtherDocBase64) ? null : Convert.FromBase64String(req.OtherDocBase64.Split(',').Last());

                var p = new[] {
                    new SqlParameter("@StaffID", req.StaffID),
                    new SqlParameter("@UserID", req.UserID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@StaffCode", req.StaffCode),
                    new SqlParameter("@FirstName", req.FirstName),
                    new SqlParameter("@LastName", (object?)req.LastName ?? DBNull.Value),
                    new SqlParameter("@FatherName", (object?)req.FatherName ?? DBNull.Value),
                    new SqlParameter("@MotherName", (object?)req.MotherName ?? DBNull.Value),
                    new SqlParameter("@Email", (object?)req.Email ?? DBNull.Value),
                    new SqlParameter("@Gender", (object?)req.Gender ?? DBNull.Value),
                    new SqlParameter("@DOB", (object?)req.DOB ?? DBNull.Value),
                    new SqlParameter("@DOJ", (object?)req.DOJ ?? DBNull.Value),
                    new SqlParameter("@MobileNo", (object?)req.MobileNo ?? DBNull.Value),
                    new SqlParameter("@EmergencyMobileNo", (object?)req.EmergencyMobileNo ?? DBNull.Value),
                    new SqlParameter("@MaritalStatus", (object?)req.MaritalStatus ?? DBNull.Value),
                    
                    new SqlParameter("@PhotoDoc", SqlDbType.VarBinary) { Value = (object?)photoBytes ?? DBNull.Value },
                    new SqlParameter("@PhotoDocType", (object?)req.PhotoDocType ?? DBNull.Value),
                    new SqlParameter("@PhotoDocName", (object?)req.PhotoDocName ?? DBNull.Value),

                    new SqlParameter("@CurrentAddress", (object?)req.CurrentAddress ?? DBNull.Value),
                    new SqlParameter("@PermanentAddress", (object?)req.PermanentAddress ?? DBNull.Value),
                    new SqlParameter("@DesignationID", (object?)req.DesignationID ?? DBNull.Value),
                    new SqlParameter("@DepartmentID", (object?)req.DepartmentID ?? DBNull.Value),
                    new SqlParameter("@Qualification", (object?)req.Qualification ?? DBNull.Value),
                    new SqlParameter("@WorkExperience", (object?)req.WorkExperience ?? DBNull.Value),
                    new SqlParameter("@Note", (object?)req.Note ?? DBNull.Value),
                    new SqlParameter("@EPFNo", (object?)req.EPFNo ?? DBNull.Value),
                    new SqlParameter("@BasicSalary", req.BasicSalary),
                    new SqlParameter("@ContractType", (object?)req.ContractType ?? DBNull.Value),
                    new SqlParameter("@WorkShift", (object?)req.WorkShift ?? DBNull.Value),
                    new SqlParameter("@WorkLocation", (object?)req.WorkLocation ?? DBNull.Value),
                    new SqlParameter("@CasualLeave", 0),
                    new SqlParameter("@SickLeave", 0),
                    new SqlParameter("@ImpWorkLeave", 0),
                    new SqlParameter("@AccountTitle", (object?)req.AccountTitle ?? DBNull.Value),
                    new SqlParameter("@BankAccountNo", (object?)req.BankAccountNo ?? DBNull.Value),
                    new SqlParameter("@BankName", (object?)req.BankName ?? DBNull.Value),
                    new SqlParameter("@IFSCCode", (object?)req.IFSCCode ?? DBNull.Value),
                    new SqlParameter("@BankBranchName", (object?)req.BankBranchName ?? DBNull.Value),
                    new SqlParameter("@FacebookURL", (object?)req.FacebookURL ?? DBNull.Value),
                    new SqlParameter("@TwitterURL", (object?)req.TwitterURL ?? DBNull.Value),
                    new SqlParameter("@LinkedinURL", (object?)req.LinkedinURL ?? DBNull.Value),
                    new SqlParameter("@InstagramURL", (object?)req.InstagramURL ?? DBNull.Value),
                    
                    new SqlParameter("@ResumeDoc", SqlDbType.VarBinary) { Value = (object?)resumeBytes ?? DBNull.Value },
                    new SqlParameter("@ResumeDocType", (object?)req.ResumeDocType ?? DBNull.Value),
                    new SqlParameter("@ResumeDocName", (object?)req.ResumeDocName ?? DBNull.Value),

                    new SqlParameter("@JoiningLetterDoc", SqlDbType.VarBinary) { Value = (object?)joiningBytes ?? DBNull.Value },
                    new SqlParameter("@JoiningLetterDocType", (object?)req.JoiningLetterDocType ?? DBNull.Value),
                    new SqlParameter("@JoiningLetterDocName", (object?)req.JoiningLetterDocName ?? DBNull.Value),

                    new SqlParameter("@ResignationLetterDoc", SqlDbType.VarBinary) { Value = (object?)resignBytes ?? DBNull.Value },
                    new SqlParameter("@ResignationLetterDocType", (object?)req.ResignationLetterDocType ?? DBNull.Value),
                    new SqlParameter("@ResignationLetterDocName", (object?)req.ResignationLetterDocName ?? DBNull.Value),

                    new SqlParameter("@OtherDoc", SqlDbType.VarBinary) { Value = (object?)otherBytes ?? DBNull.Value },
                    new SqlParameter("@OtherDocType", (object?)req.OtherDocType ?? DBNull.Value),
                    new SqlParameter("@OtherDocName", (object?)req.OtherDocName ?? DBNull.Value),

                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@DoneBy", userId),
                    new SqlParameter("@Username", req.Username),
                    new SqlParameter("@PasswordPlain", (object?)req.PasswordPlain ?? DBNull.Value),
                    new SqlParameter("@UserTypeID", req.UserTypeID),
                    new SqlParameter("@RoleIDs", string.Join(",", req.RoleIDs)),
                    new SqlParameter("@CompanyIDs", string.Join(",", req.CompanyIDs))
                };

                var dt = _db.ExecuteQuery("sp_HR_Staff_Upsert", p);
                var success = Convert.ToInt32(dt.Rows[0]["Result"]) == 1;
                var msg = dt.Rows[0]["Message"].ToString()!;
                var staffId = Convert.ToInt32(dt.Rows[0]["StaffID"]);
                
                if (success)
                {
                    if (req.StaffID == 0)
                        msg += $" | Generated Password: {req.PasswordPlain}";

                    // Sync Dynamic Leave Quotas to HR_StaffLeaveQuota table
                    int updatedCount = 0;
                    string quotaErrors = "";
                    foreach (var q in req.LeaveQuotas)
                    {
                        var quotaRes = UpsertStaffLeaveQuota(companyId, sessionId, staffId, q.LeaveTypeID, q.MaxDays, userId);
                        if (quotaRes.Success) updatedCount++;
                        else quotaErrors += $"[ID {q.LeaveTypeID}: {quotaRes.Message}] ";
                    }

                    if (updatedCount > 0) msg += $" | {updatedCount} Quotas updated.";
                    if (!string.IsNullOrEmpty(quotaErrors)) msg += $" | Quota Errors: {quotaErrors}";
                }

                return (success, msg);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private (bool Success, string Message) UpsertStaffLeaveQuota(int companyId, int sessionId, int staffId, int leaveTypeId, decimal maxDays, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@StaffID", staffId),
                    new SqlParameter("@LeaveTypeID", leaveTypeId),
                    new SqlParameter("@MaxDays", maxDays),
                    new SqlParameter("@DoneBy", userId)
                };
                _db.ExecuteNonQuery("sp_HR_StaffLeaveQuota_Upsert", p);
                return (true, "Success");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a staff member's record from the database.
        /// </summary>
        public (bool Success, string Message) DeleteStaff(int id, int userId)
        {
            try
            {
                var p = new[] { 
                    new SqlParameter("@StaffID", id), 
                    new SqlParameter("@DoneBy", userId) 
                };
                var dt = _db.ExecuteQuery("sp_HR_Staff_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
            var random = new Random();
            return "Staff@" + new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Generates a unique identification code for a new staff member based on school settings.
        /// </summary>
        public string GetNewStaffCode(int companyId, int sessionId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@EntityType", "Staff"),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId)
                };
                var dt = _db.ExecuteQuery("sp_Settings_IDAutoGen_GetNext", p);
                return dt.Rows.Count > 0 ? dt.Rows[0]["NextID"].ToString()! : "STF" + DateTime.Now.Ticks.ToString().Substring(10);
            }
            catch { return "STF" + DateTime.Now.Ticks.ToString().Substring(10); }
        }

        public (byte[] Bytes, string FileName, string ContentType) GetStaffDocument(int staffId, string docType)
        {
            try
            {
                var p = new[] { new SqlParameter("@StaffID", staffId) };
                var dt = _db.ExecuteQuery("sp_HR_Staff_GetByID", p);
                if (dt.Rows.Count == 0) return (null!, null!, null!);

                var row = dt.Rows[0];
                byte[] bytes = null!;
                string fileName = "";
                string contentType = "application/octet-stream";

                switch (docType.ToLower())
                {
                    case "resume":
                        bytes = row["ResumeDoc"] != DBNull.Value ? (byte[])row["ResumeDoc"] : null!;
                        fileName = row["ResumeDocName"]?.ToString() ?? "Resume.pdf";
                        contentType = row["ResumeDocType"]?.ToString() ?? "application/pdf";
                        break;
                    case "joiningletter":
                        bytes = row["JoiningLetterDoc"] != DBNull.Value ? (byte[])row["JoiningLetterDoc"] : null!;
                        fileName = row["JoiningLetterDocName"]?.ToString() ?? "JoiningLetter.pdf";
                        contentType = row["JoiningLetterDocType"]?.ToString() ?? "application/pdf";
                        break;
                    case "resignationletter":
                        bytes = row["ResignationLetterDoc"] != DBNull.Value ? (byte[])row["ResignationLetterDoc"] : null!;
                        fileName = row["ResignationLetterDocName"]?.ToString() ?? "ResignationLetter.pdf";
                        contentType = row["ResignationLetterDocType"]?.ToString() ?? "application/pdf";
                        break;
                    case "other":
                        bytes = row["OtherDoc"] != DBNull.Value ? (byte[])row["OtherDoc"] : null!;
                        fileName = row["OtherDocName"]?.ToString() ?? "Document.pdf";
                        contentType = row["OtherDocType"]?.ToString() ?? "application/pdf";
                        break;
                }

                return (bytes, fileName, contentType);
            }
            catch { return (null!, null!, null!); }
        }

        // --- Attendance ---

        public List<HRStaffAttendanceViewModel> GetStaffAttendance(int companyId, int sessionId, DateTime date, int? roleId)
        {
            var list = new List<HRStaffAttendanceViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@AttendanceDate", date),
                    new SqlParameter("@RoleID", (object?)roleId ?? DBNull.Value)
                };
                foreach (DataRow r in _db.ExecuteQuery("sp_HR_StaffAttendance_GetByDate", p).Rows)
                {
                    list.Add(new HRStaffAttendanceViewModel
                    {
                        StaffID = Convert.ToInt32(r["StaffID"]),
                        StaffCode = r["StaffCode"].ToString()!,
                        StaffName = r["StaffName"].ToString()!,
                        RoleName = r["RoleName"].ToString()!,
                        StaffAttendanceID = Convert.ToInt32(r["StaffAttendanceID"]),
                        StaffAttendance = r["StaffAttendance"].ToString()!,
                        StaffAttendanceSource = r["StaffAttendanceSource"].ToString()!,
                        StaffAttendanceNote = r["StaffAttendanceNote"].ToString()!,
                        LastUpdated = r["LastUpdated"] == DBNull.Value ? null : Convert.ToDateTime(r["LastUpdated"])
                    });
                }
            }
            catch (Exception) { }
            return list;
        }

        public (bool Success, string Message) SaveStaffAttendance(HRStaffAttendanceUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@StaffID", req.StaffID),
                    new SqlParameter("@AttendanceDate", req.AttendanceDate),
                    new SqlParameter("@Attendance", req.Attendance),
                    new SqlParameter("@Source", req.Source),
                    new SqlParameter("@Note", req.Note),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_HR_StaffAttendance_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        // --- Payroll ---

        public List<HRPayrollViewModel> GetAllPayroll(int companyId, int sessionId, int month, int year, int? roleId)
        {
            var list = new List<HRPayrollViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@Month", month),
                    new SqlParameter("@Year", year),
                    new SqlParameter("@RoleID", (object?)roleId ?? DBNull.Value)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_HR_Payroll_GetAll", p).Rows)
                    list.Add(MapPayroll(row));
            }
            catch (Exception) { }
            return list;
        }

        public (bool Success, string Message) GeneratePayroll(HRPayrollGenerateRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@StaffID", req.StaffID),
                    new SqlParameter("@Month", req.Month),
                    new SqlParameter("@Year", req.Year),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_HR_Payroll_Generate", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public HRPayrollGenerationViewModel GetPayrollGenerationData(int staffId, int month, int year, int companyId, int sessionId)
        {
            var model = new HRPayrollGenerationViewModel
            {
                Staff = GetStaffByID(staffId) ?? new HRStaffViewModel(),
                Month = month,
                Year = year,
                AttendanceHistory = new List<HRAttendanceSummary>()
            };

            model.BasicSalary = model.Staff.BasicSalary;

            // Fetch attendance for the requested month and the previous month (to match UI)
            int prevMonth = month == 1 ? 12 : month - 1;
            int prevYear = month == 1 ? year - 1 : year;

            model.AttendanceHistory.Add(FetchAttendanceSummary(staffId, prevMonth, prevYear, companyId));
            model.AttendanceHistory.Add(FetchAttendanceSummary(staffId, month, year, companyId));

            return model;
        }

        private HRAttendanceSummary FetchAttendanceSummary(int staffId, int month, int year, int companyId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@StaffID", staffId),
                    new SqlParameter("@Month", month),
                    new SqlParameter("@Year", year),
                    new SqlParameter("@CompanyID", companyId)
                };
                var dt = _db.ExecuteQuery("sp_HR_Attendance_GetSummary", p);
                if (dt.Rows.Count > 0)
                {
                    var r = dt.Rows[0];
                    return new HRAttendanceSummary
                    {
                        Month = month,
                        MonthName = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(month),
                        Year = year,
                        Present = Convert.ToInt32(r["Present"]),
                        Late = Convert.ToInt32(r["Late"]),
                        Absent = Convert.ToInt32(r["Absent"]),
                        HalfDay = Convert.ToInt32(r["HalfDay"]),
                        Holiday = Convert.ToInt32(r["Holiday"]),
                        Leave = Convert.ToInt32(r["Leave"])
                    };
                }
            }
            catch { }
            return new HRAttendanceSummary { Month = month, MonthName = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(month), Year = year };
        }

        public (bool Success, string Message) SaveDetailedPayroll(HRPayrollSaveRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                SqlParameter payrollIdParam = new SqlParameter("@PayrollID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var p = new SqlParameter[] {
                    payrollIdParam,
                    new SqlParameter("@StaffID", req.StaffID),
                    new SqlParameter("@Month", req.Month),
                    new SqlParameter("@Year", req.Year),
                    new SqlParameter("@BasicSalary", req.BasicSalary),
                    new SqlParameter("@TotalEarnings", req.TotalEarnings),
                    new SqlParameter("@TotalDeductions", req.TotalDeductions),
                    new SqlParameter("@NetSalary", req.NetSalary),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@UserID", userId)
                };
                
                _db.ExecuteNonQuery("sp_HR_Payroll_SaveDetailed", p);
                int payrollId = (payrollIdParam.Value == DBNull.Value) ? 0 : Convert.ToInt32(payrollIdParam.Value);

                if (payrollId > 0)
                {
                    foreach (var detail in req.Details)
                    {
                        var pd = new SqlParameter[] {
                            new SqlParameter("@PayrollID", payrollId),
                            new SqlParameter("@ComponentName", detail.ComponentName),
                            new SqlParameter("@ComponentType", detail.ComponentType),
                            new SqlParameter("@Amount", detail.Amount),
                            new SqlParameter("@UserID", userId)
                        };
                        _db.ExecuteNonQuery("sp_HR_PayrollDetail_Insert", pd);
                    }
                    return (true, "Payroll saved successfully.");
                }
                return (false, "Failed to save payroll.");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) MarkAsPaid(HRPayrollPaymentRequest req, int userId)
        {
            try
            {
                var p = new SqlParameter[] {
                    new SqlParameter("@PayrollID", req.PayrollID),
                    new SqlParameter("@PaymentMode", req.PaymentMode),
                    new SqlParameter("@PaymentDate", req.PaymentDate),
                    new SqlParameter("@Note", (object?)req.Note ?? DBNull.Value),
                    new SqlParameter("@UserID", userId)
                };
                _db.ExecuteNonQuery("sp_HR_Payroll_MarkAsPaid", p);
                return (true, "Payment recorded successfully.");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static HRPayrollViewModel MapPayroll(DataRow r)
        {
            return new HRPayrollViewModel
            {
                PayrollID = Convert.ToInt32(r["PayrollID"]),
                StaffID = Convert.ToInt32(r["StaffID"]),
                StaffName = r["StaffName"].ToString()!,
                StaffCode = r["StaffCode"].ToString()!,
                RoleName = r.Table.Columns.Contains("RoleName") ? r["RoleName"].ToString()! : "",
                DepartmentName = r.Table.Columns.Contains("DepartmentName") ? r["DepartmentName"].ToString()! : "",
                DesignationName = r.Table.Columns.Contains("DesignationName") ? r["DesignationName"].ToString()! : "",
                MobileNo = r.Table.Columns.Contains("MobileNo") ? r["MobileNo"].ToString()! : "",
                Month = Convert.ToInt32(r["Month"]),
                Year = Convert.ToInt32(r["Year"]),
                BasicSalary = Convert.ToDecimal(r["BasicSalary"]),
                TotalEarnings = Convert.ToDecimal(r["TotalEarnings"]),
                TotalDeductions = Convert.ToDecimal(r["TotalDeductions"]),
                NetSalary = Convert.ToDecimal(r["NetSalary"]),
                AttendanceDays = Convert.ToDecimal(r["AttendanceDays"]),
                Status = r["Status"].ToString()!,
                PaymentMode = r["PaymentMode"]?.ToString(),
                PaymentDate = r["PaymentDate"] == DBNull.Value ? null : Convert.ToDateTime(r["PaymentDate"]),
                Note = r["Note"]?.ToString()
            };
        }

        // --- Apply Leave ---

        public List<HRApplyLeaveViewModel> GetAllApplyLeave(int companyId, int sessionId)
        {
            var list = new List<HRApplyLeaveViewModel>();
            try
            {
                var p = new[] { new SqlParameter("@CompanyID", companyId), new SqlParameter("@SessionID", sessionId) };
                foreach (DataRow row in _db.ExecuteQuery("sp_HR_ApplyLeave_GetAll", p).Rows)
                    list.Add(MapApplyLeave(row));
            }
            catch { }
            return list;
        }

        public HRApplyLeaveViewModel? GetApplyLeaveByID(int id)
        {
            var p = new[] { new SqlParameter("@ApplyLeaveID", id) };
            var dt = _db.ExecuteQuery("sp_HR_ApplyLeave_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapApplyLeave(dt.Rows[0]);
        }

        public (bool Success, string Message) UpsertApplyLeave(HRApplyLeaveUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                // Fallback: If companyId/sessionId is 0, try to get them from the staff record
                if (companyId <= 0 || sessionId <= 0)
                {
                    var staff = GetStaffByID(req.StaffID);
                    if (staff != null)
                    {
                        if (companyId <= 0) companyId = staff.CompanyID;
                        if (sessionId <= 0) sessionId = staff.SessionID;
                    }
                }

                byte[]? attachmentBytes = null;
                if (!string.IsNullOrEmpty(req.AttachmentBase64))
                {
                    var base64Data = req.AttachmentBase64.Contains(",") ? req.AttachmentBase64.Split(',')[1] : req.AttachmentBase64;
                    attachmentBytes = Convert.FromBase64String(base64Data);
                }

                var p = new[] {
                    new SqlParameter("@ApplyLeaveID", req.ApplyLeaveID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@StaffID", req.StaffID),
                    new SqlParameter("@LeaveTypeID", req.LeaveTypeID),
                    new SqlParameter("@FromDate", req.FromDate),
                    new SqlParameter("@ToDate", req.ToDate),
                    new SqlParameter("@Reason", (object?)req.Reason ?? DBNull.Value),
                    new SqlParameter("@Status", req.Status ?? "Pending"),
                    new SqlParameter("@Note", (object?)req.Note ?? DBNull.Value),
                    new SqlParameter("@AttachmentDoc", SqlDbType.VarBinary) { Value = (object?)attachmentBytes ?? DBNull.Value },
                    new SqlParameter("@AttachmentDocType", (object?)req.AttachmentDocType ?? DBNull.Value),
                    new SqlParameter("@AttachmentDocName", (object?)req.AttachmentDocName ?? DBNull.Value),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@IPAddress", DBNull.Value) // Optional: pass real IP if available
                };
                var dt = _db.ExecuteQuery("sp_HR_ApplyLeave_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (byte[] Bytes, string FileName, string ContentType) GetApplyLeaveDocument(int id)
        {
            try
            {
                var p = new[] { new SqlParameter("@ApplyLeaveID", id) };
                var dt = _db.ExecuteQuery("sp_HR_ApplyLeave_GetByID", p);
                if (dt.Rows.Count == 0) return (null!, null!, null!);

                var row = dt.Rows[0];
                if (row["AttachmentFile"] == DBNull.Value) return (null!, null!, null!);

                byte[] bytes = (byte[])row["AttachmentFile"];
                string fileName = row["AttachmentDocName"]?.ToString() ?? "Attachment.pdf";
                string contentType = row["AttachmentDocType"]?.ToString() ?? "application/pdf";

                return (bytes, fileName, contentType);
            }
            catch { return (null!, null!, null!); }
        }

        public (bool Success, string Message) DeleteApplyLeave(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@ApplyLeaveID", id), new SqlParameter("@UserID", userId) };
                var dt = _db.ExecuteQuery("sp_HR_ApplyLeave_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) UpdateApplyLeaveStatus(HRApplyLeaveStatusUpdateRequest req, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@ApplyLeaveID", req.ApplyLeaveID),
                    new SqlParameter("@Status", req.Status),
                    new SqlParameter("@Note", (object?)req.Note ?? DBNull.Value),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_HR_ApplyLeave_StatusUpdate", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static HRApplyLeaveViewModel MapApplyLeave(DataRow r) => new()
        {
            ApplyLeaveID = Convert.ToInt32(r["ApplyLeaveID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            StaffID = Convert.ToInt32(r["StaffID"]),
            StaffName = r.Table.Columns.Contains("StaffName") ? r["StaffName"].ToString()! : "Unknown",
            StaffCode = r.Table.Columns.Contains("StaffCode") ? r["StaffCode"].ToString()! : "-",
            LeaveTypeID = Convert.ToInt32(r["LeaveTypeID"]),
            LeaveTypeName = r.Table.Columns.Contains("LeaveTypeName") ? r["LeaveTypeName"].ToString()! : "Unknown",
            ApplyDate = Convert.ToDateTime(r["ApplyDate"]),
            FromDate = Convert.ToDateTime(r["FromDate"]),
            ToDate = Convert.ToDateTime(r["ToDate"]),
            Reason = r.Table.Columns.Contains("Reason") && r["Reason"] != DBNull.Value ? r["Reason"].ToString() : null,
            Status = r["Status"].ToString()!,
            ApprovedBy = r.Table.Columns.Contains("ApprovedBy") && r["ApprovedBy"] != DBNull.Value ? Convert.ToInt32(r["ApprovedBy"]) : (int?)null,
            ApprovedByName = r.Table.Columns.Contains("ApprovedByName") ? (r["ApprovedByName"]?.ToString() ?? "-") : "-",
            AttachmentDocType = r.Table.Columns.Contains("AttachmentDocType") && r["AttachmentDocType"] != DBNull.Value ? r["AttachmentDocType"].ToString() : null,
            AttachmentDocName = r.Table.Columns.Contains("AttachmentDocName") && r["AttachmentDocName"] != DBNull.Value ? r["AttachmentDocName"].ToString() : null,
            Note = r.Table.Columns.Contains("Note") && r["Note"] != DBNull.Value ? r["Note"].ToString() : null
        };

        // --- Mapping Helpers ---
        private static HRDesignationViewModel MapDesignation(DataRow r) => new()
        {
            HRDesignationID = Convert.ToInt32(r["HRDesignationID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            DesignationName = r["DesignationName"].ToString()!,
            IsActive = Convert.ToBoolean(r["IsActive"]),
            IsDelete = Convert.ToBoolean(r["IsDelete"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"]),
            CreatedBy = Convert.ToInt32(r["CreatedBy"]),
            ModifiedOn = r["ModifiedOn"] == DBNull.Value ? null : Convert.ToDateTime(r["ModifiedOn"]),
            ModifiedBy = r["ModifiedBy"] == DBNull.Value ? null : Convert.ToInt32(r["ModifiedBy"])
        };

        private static HRDepartmentViewModel MapDepartment(DataRow r) => new()
        {
            DepartmentID = Convert.ToInt32(r["DepartmentID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            DepartmentName = r["DepartmentName"].ToString()!,
            IsActive = Convert.ToBoolean(r["IsActive"]),
            IsDelete = Convert.ToBoolean(r["IsDelete"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"]),
            CreatedBy = Convert.ToInt32(r["CreatedBy"]),
            ModifiedOn = r["ModifiedOn"] == DBNull.Value ? null : Convert.ToDateTime(r["ModifiedOn"]),
            ModifiedBy = r["ModifiedBy"] == DBNull.Value ? null : Convert.ToInt32(r["ModifiedBy"])
        };

        private static HRLeaveTypeViewModel MapLeaveType(DataRow r) => new()
        {
            LeaveTypeID = Convert.ToInt32(r["LeaveTypeID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            LeaveTypeName = r["LeaveTypeName"].ToString()!,
            IsActive = Convert.ToBoolean(r["IsActive"]),
            IsDelete = Convert.ToBoolean(r["IsDelete"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"]),
            CreatedBy = Convert.ToInt32(r["CreatedBy"]),
            ModifiedOn = r["ModifiedOn"] == DBNull.Value ? null : Convert.ToDateTime(r["ModifiedOn"]),
            ModifiedBy = r["ModifiedBy"] == DBNull.Value ? null : Convert.ToInt32(r["ModifiedBy"])
        };

        private static HRStaffViewModel MapStaff(DataRow r) => new()
        {
            StaffID = Convert.ToInt32(r["StaffID"]),
            UserID = r.Table.Columns.Contains("UserID") && r["UserID"] != DBNull.Value ? Convert.ToInt32(r["UserID"]) : 
                     (r.Table.Columns.Contains("UserId") && r["UserId"] != DBNull.Value ? Convert.ToInt32(r["UserId"]) : null),
            StaffCode = r.Table.Columns.Contains("StaffCode") ? r["StaffCode"].ToString()! : "",
            FirstName = r.Table.Columns.Contains("FirstName") ? r["FirstName"].ToString()! : "",
            LastName = r.Table.Columns.Contains("LastName") ? r["LastName"]?.ToString() ?? "" : "",
            FatherName = r.Table.Columns.Contains("FatherName") ? r["FatherName"]?.ToString() ?? "" : "",
            MotherName = r.Table.Columns.Contains("MotherName") ? r["MotherName"]?.ToString() ?? "" : "",
            Email = r.Table.Columns.Contains("Email") ? r["Email"]?.ToString() ?? "" : "",
            MobileNo = r.Table.Columns.Contains("MobileNo") ? r["MobileNo"]?.ToString() ?? "" : "",
            EmergencyMobileNo = r.Table.Columns.Contains("EmergencyMobileNo") ? r["EmergencyMobileNo"]?.ToString() ?? "" : "",
            DOB = r.Table.Columns.Contains("DOB") && r["DOB"] != DBNull.Value ? Convert.ToDateTime(r["DOB"]) : null,
            DOJ = r.Table.Columns.Contains("DOJ") && r["DOJ"] != DBNull.Value ? Convert.ToDateTime(r["DOJ"]) : null,
            Gender = r.Table.Columns.Contains("Gender") ? r["Gender"]?.ToString() ?? "" : "",
            MaritalStatus = r.Table.Columns.Contains("MaritalStatus") ? r["MaritalStatus"]?.ToString() ?? "" : "",
            
            PhotoDoc = r.Table.Columns.Contains("PhotoDoc") && r["PhotoDoc"] != DBNull.Value ? (byte[])r["PhotoDoc"] : null,
            PhotoDocType = r.Table.Columns.Contains("PhotoDocType") ? r["PhotoDocType"]?.ToString() : "",
            PhotoDocName = r.Table.Columns.Contains("PhotoDocName") ? r["PhotoDocName"]?.ToString() : "",

            CurrentAddress = r.Table.Columns.Contains("CurrentAddress") ? r["CurrentAddress"]?.ToString() ?? "" : "",
            PermanentAddress = r.Table.Columns.Contains("PermanentAddress") ? r["PermanentAddress"]?.ToString() ?? "" : "",
            DesignationID = r.Table.Columns.Contains("DesignationID") && r["DesignationID"] != DBNull.Value ? Convert.ToInt32(r["DesignationID"]) : null,
            DesignationName = r.Table.Columns.Contains("DesignationName") ? r["DesignationName"]?.ToString() ?? "" : "",
            DepartmentID = r.Table.Columns.Contains("DepartmentID") && r["DepartmentID"] != DBNull.Value ? Convert.ToInt32(r["DepartmentID"]) : null,
            DepartmentName = r.Table.Columns.Contains("DepartmentName") ? r["DepartmentName"]?.ToString() ?? "" : "",
            Qualification = r.Table.Columns.Contains("Qualification") ? r["Qualification"]?.ToString() ?? "" : "",
            WorkExperience = r.Table.Columns.Contains("WorkExperience") ? r["WorkExperience"]?.ToString() ?? "" : "",
            Note = r.Table.Columns.Contains("Note") ? r["Note"]?.ToString() ?? "" : "",
            EPFNo = r.Table.Columns.Contains("EPFNo") ? r["EPFNo"]?.ToString() ?? "" : "",
            BasicSalary = r.Table.Columns.Contains("BasicSalary") ? Convert.ToDecimal(r["BasicSalary"]) : 0,
            ContractType = r.Table.Columns.Contains("ContractType") ? r["ContractType"]?.ToString() ?? "" : "",
            WorkShift = r.Table.Columns.Contains("WorkShift") ? r["WorkShift"]?.ToString() ?? "" : "",
            WorkLocation = r.Table.Columns.Contains("WorkLocation") ? r["WorkLocation"]?.ToString() ?? "" : "",
            CasualLeave = r.Table.Columns.Contains("CasualLeave") && r["CasualLeave"] != DBNull.Value ? Convert.ToInt32(r["CasualLeave"]) : 
                          (r.Table.Columns.Contains("Casual_Leave") && r["Casual_Leave"] != DBNull.Value ? Convert.ToInt32(r["Casual_Leave"]) : 0),
            SickLeave = r.Table.Columns.Contains("SickLeave") && r["SickLeave"] != DBNull.Value ? Convert.ToInt32(r["SickLeave"]) : 
                        (r.Table.Columns.Contains("Sick_Leave") && r["Sick_Leave"] != DBNull.Value ? Convert.ToInt32(r["Sick_Leave"]) : 0),
            ImpWorkLeave = r.Table.Columns.Contains("ImpWorkLeave") && r["ImpWorkLeave"] != DBNull.Value ? Convert.ToInt32(r["ImpWorkLeave"]) : 
                           (r.Table.Columns.Contains("Imp_Work_Leave") && r["Imp_Work_Leave"] != DBNull.Value ? Convert.ToInt32(r["Imp_Work_Leave"]) : 
                           (r.Table.Columns.Contains("ImpWork") && r["ImpWork"] != DBNull.Value ? Convert.ToInt32(r["ImpWork"]) : 0)),
            AccountTitle = r.Table.Columns.Contains("AccountTitle") ? r["AccountTitle"]?.ToString() ?? "" : "",
            BankAccountNo = r.Table.Columns.Contains("BankAccountNo") ? r["BankAccountNo"]?.ToString() ?? "" : "",
            BankName = r.Table.Columns.Contains("BankName") ? r["BankName"]?.ToString() ?? "" : "",
            IFSCCode = r.Table.Columns.Contains("IFSCCode") ? r["IFSCCode"]?.ToString() ?? "" : "",
            BankBranchName = r.Table.Columns.Contains("BankBranchName") ? r["BankBranchName"]?.ToString() ?? "" : "",
            FacebookURL = r.Table.Columns.Contains("FacebookURL") ? r["FacebookURL"]?.ToString() ?? "" : "",
            TwitterURL = r.Table.Columns.Contains("TwitterURL") ? r["TwitterURL"]?.ToString() ?? "" : "",
            LinkedinURL = r.Table.Columns.Contains("LinkedinURL") ? r["LinkedinURL"]?.ToString() ?? "" : "",
            InstagramURL = r.Table.Columns.Contains("InstagramURL") ? r["InstagramURL"]?.ToString() ?? "" : "",
            
            ResumeDocName = r.Table.Columns.Contains("ResumeDocName") ? r["ResumeDocName"]?.ToString() : "",
            JoiningLetterDocName = r.Table.Columns.Contains("JoiningLetterDocName") ? r["JoiningLetterDocName"]?.ToString() : "",
            ResignationLetterDocName = r.Table.Columns.Contains("ResignationLetterDocName") ? r["ResignationLetterDocName"]?.ToString() : "",
            OtherDocName = r.Table.Columns.Contains("OtherDocName") ? r["OtherDocName"]?.ToString() : "",

            IsActive = r.Table.Columns.Contains("IsActive") ? Convert.ToBoolean(r["IsActive"]) : true,
            IsDelete = r.Table.Columns.Contains("IsDelete") ? Convert.ToBoolean(r["IsDelete"]) : false,
            CreatedBy = r.Table.Columns.Contains("CreatedBy") ? Convert.ToInt32(r["CreatedBy"]) : 0,
            CreatedOn = r.Table.Columns.Contains("CreatedOn") ? Convert.ToDateTime(r["CreatedOn"]) : DateTime.MinValue,
            ModifiedBy = r.Table.Columns.Contains("ModifiedBy") && r["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(r["ModifiedBy"]) : null,
            ModifiedOn = r.Table.Columns.Contains("ModifiedOn") && r["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(r["ModifiedOn"]) : null,
            
            Username = r.Table.Columns.Contains("Username") ? r["Username"]?.ToString() ?? "" : "",
            UserTypeID = r.Table.Columns.Contains("UserTypeID") ? Convert.ToInt32(r["UserTypeID"]) : 0,
            RoleName = r.Table.Columns.Contains("RoleName") ? r["RoleName"]?.ToString() ?? "" : 
                       (r.Table.Columns.Contains("RoleNames") ? r["RoleNames"]?.ToString() ?? "" : 
                       (r.Table.Columns.Contains("UserRole") ? r["UserRole"]?.ToString() ?? "" : 
                       (r.Table.Columns.Contains("Role") ? r["Role"]?.ToString() ?? "" : ""))),
            
            CompanyID = r.Table.Columns.Contains("CompanyID") ? Convert.ToInt32(r["CompanyID"]) : 
                        (r.Table.Columns.Contains("CompanyId") ? Convert.ToInt32(r["CompanyId"]) : 0),
            SessionID = r.Table.Columns.Contains("SessionID") ? Convert.ToInt32(r["SessionID"]) : 
                        (r.Table.Columns.Contains("SessionId") ? Convert.ToInt32(r["SessionId"]) : 0)
        };
    }
}
