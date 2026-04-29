using System;
using System.Collections.Generic;

namespace SchoolERP.Net.Models
{
    public class HRDesignationViewModel
    {
        public int HRDesignationID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class HRDesignationUpsertRequest
    {
        public int HRDesignationID { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class HRDesignationPageViewModel
    {
        public List<HRDesignationViewModel> Items { get; set; } = new();
    }

    public class HRDepartmentViewModel
    {
        public int DepartmentID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class HRDepartmentUpsertRequest
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class HRDepartmentPageViewModel
    {
        public List<HRDepartmentViewModel> Items { get; set; } = new();
    }

    public class HRLeaveTypeViewModel
    {
        public int LeaveTypeID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class HRLeaveTypeUpsertRequest
    {
        public int LeaveTypeID { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class HRLeaveTypePageViewModel
    {
        public List<HRLeaveTypeViewModel> Items { get; set; } = new();
    }
    
    public class HRStaffViewModel
    {
        public int StaffID { get; set; }
        public int? UserID { get; set; }
        public string StaffCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmergencyMobileNo { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public DateTime? DOJ { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        
        // Photo
        public byte[]? PhotoDoc { get; set; }
        public string? PhotoDocType { get; set; }
        public string? PhotoDocName { get; set; }

        public string CurrentAddress { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public int? DesignationID { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string WorkExperience { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        // Payroll
        public string EPFNo { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; }
        public string ContractType { get; set; } = string.Empty;
        public string WorkShift { get; set; } = string.Empty;
        public string WorkLocation { get; set; } = string.Empty;

        // Leaves
        public int CasualLeave { get; set; }
        public int SickLeave { get; set; }
        public int ImpWorkLeave { get; set; }

        // Bank
        public string AccountTitle { get; set; } = string.Empty;
        public string BankAccountNo { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string IFSCCode { get; set; } = string.Empty;
        public string BankBranchName { get; set; } = string.Empty;

        // Social
        public string FacebookURL { get; set; } = string.Empty;
        public string TwitterURL { get; set; } = string.Empty;
        public string LinkedinURL { get; set; } = string.Empty;
        public string InstagramURL { get; set; } = string.Empty;

        // Documents (Metadata only for VM)
        public string? ResumeDocName { get; set; }
        public string? JoiningLetterDocName { get; set; }
        public string? ResignationLetterDocName { get; set; }
        public string? OtherDocName { get; set; }

        public bool IsActive { get; set; }
        public string Username { get; set; } = string.Empty;
        public int UserTypeID { get; set; }
        public List<int> RoleIDs { get; set; } = new();
        public List<int> CompanyIDs { get; set; } = new();
        public string RoleName { get; set; } = string.Empty;
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public bool IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class HRStaffUpsertRequest
    {
        public int StaffID { get; set; }
        public int UserID { get; set; }
        public string StaffCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmergencyMobileNo { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public DateTime? DOJ { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        
        // Photo
        public string? PhotoBase64 { get; set; }
        public string? PhotoDocType { get; set; }
        public string? PhotoDocName { get; set; }

        public string CurrentAddress { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public int? DesignationID { get; set; }
        public int? DepartmentID { get; set; }
        public string Qualification { get; set; } = string.Empty;
        public string WorkExperience { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        // Payroll
        public string EPFNo { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; }
        public string ContractType { get; set; } = string.Empty;
        public string WorkShift { get; set; } = string.Empty;
        public string WorkLocation { get; set; } = string.Empty;

        // Leaves
        public int CasualLeave { get; set; }
        public int SickLeave { get; set; }
        public int ImpWorkLeave { get; set; }

        // Bank
        public string AccountTitle { get; set; } = string.Empty;
        public string BankAccountNo { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string IFSCCode { get; set; } = string.Empty;
        public string BankBranchName { get; set; } = string.Empty;

        // Social
        public string FacebookURL { get; set; } = string.Empty;
        public string TwitterURL { get; set; } = string.Empty;
        public string LinkedinURL { get; set; } = string.Empty;
        public string InstagramURL { get; set; } = string.Empty;

        // Documents (Base64 for Upload)
        public string? ResumeBase64 { get; set; }
        public string? ResumeDocType { get; set; }
        public string? ResumeDocName { get; set; }

        public string? JoiningLetterBase64 { get; set; }
        public string? JoiningLetterDocType { get; set; }
        public string? JoiningLetterDocName { get; set; }

        public string? ResignationLetterBase64 { get; set; }
        public string? ResignationLetterDocType { get; set; }
        public string? ResignationLetterDocName { get; set; }

        public string? OtherDocBase64 { get; set; }
        public string? OtherDocType { get; set; }
        public string? OtherDocName { get; set; }

        public bool IsActive { get; set; }

        // User Management Fields
        public string Username { get; set; } = string.Empty;
        public string? PasswordPlain { get; set; }
        public int UserTypeID { get; set; }
        public List<int> RoleIDs { get; set; } = new();
        public List<int> CompanyIDs { get; set; } = new();
    }

    public class HRStaffPageViewModel
    {
        public string NewStaffCode { get; set; } = string.Empty;
        public List<HRStaffViewModel> StaffList { get; set; } = new();
        public List<HRDesignationViewModel> Designations { get; set; } = new();
        public List<HRDepartmentViewModel> Departments { get; set; } = new();
        public List<MstRoleViewModel> Roles { get; set; } = new();
        public List<MstUserTypeViewModel> UserTypes { get; set; } = new();
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public HRStaffViewModel? EditStaff { get; set; }
    }
}
