using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class HumanResourceService : IHumanResourceService
    {
        private readonly SqlHelper _db;
        public HumanResourceService(SqlHelper db) => _db = db;

        // --- Designation ---
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

        public HRDesignationViewModel? GetDesignationByID(int id)
        {
            var p = new[] { new SqlParameter("@HRDesignationID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_HRDesignation_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapDesignation(dt.Rows[0]);
        }

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

        public HRDepartmentViewModel? GetDepartmentByID(int id)
        {
            var p = new[] { new SqlParameter("@DepartmentID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_Department_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapDepartment(dt.Rows[0]);
        }

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

        public HRLeaveTypeViewModel? GetLeaveTypeByID(int id)
        {
            var p = new[] { new SqlParameter("@LeaveTypeID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_LeaveType_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapLeaveType(dt.Rows[0]);
        }

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

                    foreach (DataRow row in dt.Rows)
                        list.Add(MapStaff(row));
                }
            }
            catch (Exception) { }
            return list;
        }

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
                    staff.CompanyIDs.Add(Convert.ToInt32(r["CompanyID"]));
            }

            return staff;
        }

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
            UserID = r.Table.Columns.Contains("UserID") && r["UserID"] != DBNull.Value ? Convert.ToInt32(r["UserID"]) : null,
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
            RoleName = r.Table.Columns.Contains("RoleName") ? r["RoleName"]?.ToString() ?? "" : "",
            
            CompanyID = r.Table.Columns.Contains("CompanyID") ? Convert.ToInt32(r["CompanyID"]) : 0,
            SessionID = r.Table.Columns.Contains("SessionID") ? Convert.ToInt32(r["SessionID"]) : 0
        };
    }
}
