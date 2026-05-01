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

            // Fallback: Ensure the primary CompanyID is also in the list
            if (staff.CompanyID > 0 && !staff.CompanyIDs.Contains(staff.CompanyID))
                staff.CompanyIDs.Add(staff.CompanyID);

            // NEW: Fetch roles and companies assigned to the linked User account (Source of Truth)
            if (staff.UserID.HasValue && staff.UserID > 0)
            {
                try
                {
                    var userParams = new[] { new SqlParameter("@UserID", staff.UserID.Value) };
                    
                    // Sync Roles
                    var userRolesDt = _db.ExecuteQuery("sp_UserRoles_GetByUser", userParams);
                    foreach (DataRow r in userRolesDt.Rows)
                    {
                        int rId = Convert.ToInt32(r["RoleID"]);
                        if (rId > 0 && !staff.RoleIDs.Contains(rId))
                            staff.RoleIDs.Add(rId);
                    }

                    // Sync Companies
                    var userCompDt = _db.ExecuteQuery("sp_UserCompanies_GetByUser", userParams);
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
                }
                catch (Exception) { /* Handle or log if needed */ }
            }

            return staff;
        }

        /// <summary>
        /// Saves or updates a staff member's record in the database, including personal info, documents, and system permissions.
        /// </summary>
        public (bool Success, string Message) UpsertStaff(HRStaffUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
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
                    new SqlParameter("@CasualLeave", req.CasualLeave),
                    new SqlParameter("@SickLeave", req.SickLeave),
                    new SqlParameter("@ImpWorkLeave", req.ImpWorkLeave),
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
                
                if (success && req.StaffID == 0)
                {
                    msg += $" | Generated Password: {req.PasswordPlain}";
                }

                return (success, msg);
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
                        StaffAttendanceID = r["StaffAttendanceID"] == DBNull.Value ? 0 : Convert.ToInt32(r["StaffAttendanceID"]),
                        StaffAttendance = r["StaffAttendance"].ToString()!,
                        StaffAttendanceSource = r["StaffAttendanceSource"].ToString()!,
                        StaffAttendanceNote = r["StaffAttendanceNote"].ToString()!,
                        LastUpdated = r["LastUpdated"] == DBNull.Value ? null : Convert.ToDateTime(r["LastUpdated"])
                    });
                }
            }
            catch { }
            return list;
        }

        public (bool Success, string Message) SaveStaffAttendance(HRStaffAttendanceUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@StaffID", req.StaffID),
                    new SqlParameter("@AttendanceDate", req.AttendanceDate),
                    new SqlParameter("@Attendance", req.Attendance),
                    new SqlParameter("@Source", req.Source),
                    new SqlParameter("@Note", req.Note),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_HR_StaffAttendance_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
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
                byte[]? attachment = string.IsNullOrEmpty(req.AttachmentBase64) ? null : Convert.FromBase64String(req.AttachmentBase64.Split(',').Last());

                var p = new[] {
                    new SqlParameter("@ApplyLeaveID", req.ApplyLeaveID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@StaffID", req.StaffID),
                    new SqlParameter("@LeaveTypeID", req.LeaveTypeID),
                    new SqlParameter("@FromDate", req.FromDate),
                    new SqlParameter("@ToDate", req.ToDate),
                    new SqlParameter("@Reason", (object?)req.Reason ?? DBNull.Value),
                    new SqlParameter("@Status", req.Status),
                    new SqlParameter("@Note", (object?)req.Note ?? DBNull.Value),
                    new SqlParameter("@AttachmentDoc", (object?)attachment ?? DBNull.Value),
                    new SqlParameter("@AttachmentDocType", (object?)req.AttachmentDocType ?? DBNull.Value),
                    new SqlParameter("@AttachmentDocName", (object?)req.AttachmentDocName ?? DBNull.Value),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_HR_ApplyLeave_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
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

        private static HRApplyLeaveViewModel MapApplyLeave(DataRow r) => new()
        {
            ApplyLeaveID = Convert.ToInt32(r["ApplyLeaveID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            StaffID = Convert.ToInt32(r["StaffID"]),
            StaffName = r["StaffName"].ToString()!,
            StaffCode = r["StaffCode"].ToString()!,
            LeaveTypeID = Convert.ToInt32(r["LeaveTypeID"]),
            LeaveTypeName = r["LeaveTypeName"].ToString()!,
            ApplyDate = Convert.ToDateTime(r["ApplyDate"]),
            FromDate = Convert.ToDateTime(r["FromDate"]),
            ToDate = Convert.ToDateTime(r["ToDate"]),
            Reason = r["Reason"] == DBNull.Value ? null : r["Reason"].ToString(),
            Status = r["Status"].ToString()!,
            ApprovedBy = r["ApprovedBy"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["ApprovedBy"]),
            ApprovedByName = r["ApprovedByName"]?.ToString() ?? "-",
            AttachmentDocType = r["AttachmentDocType"] == DBNull.Value ? null : r["AttachmentDocType"].ToString(),
            AttachmentDocName = r["AttachmentDocName"] == DBNull.Value ? null : r["AttachmentDocName"].ToString(),
            Note = r["Note"] == DBNull.Value ? null : r["Note"].ToString()
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
            CasualLeave = r.Table.Columns.Contains("CasualLeave") ? Convert.ToInt32(r["CasualLeave"]) : 0,
            SickLeave = r.Table.Columns.Contains("SickLeave") ? Convert.ToInt32(r["SickLeave"]) : 0,
            ImpWorkLeave = r.Table.Columns.Contains("ImpWorkLeave") ? Convert.ToInt32(r["ImpWorkLeave"]) : 0,
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
