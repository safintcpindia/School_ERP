using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolERP.Net.Models
{
    // ─── PURPOSE ────────────────────────────────────────────────
    public class MstFOPurposeViewModel
    {
        public int PurposeID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class MstFOPurposeUpsertRequest
    {
        public int PurposeID { get; set; }

        [Required(ErrorMessage = "Purpose name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ─── COMPLAINT TYPE ─────────────────────────────────────────
    public class MstFOComplaintTypeViewModel
    {
        public int ComplaintTypeID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class MstFOComplaintTypeUpsertRequest
    {
        public int ComplaintTypeID { get; set; }

        [Required(ErrorMessage = "Complaint Type name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ─── SOURCE ─────────────────────────────────────────────────
    public class MstFOSourceViewModel
    {
        public int SourceID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class MstFOSourceUpsertRequest
    {
        public int SourceID { get; set; }

        [Required(ErrorMessage = "Source name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ─── REFERENCE ──────────────────────────────────────────────
    public class MstFOReferenceViewModel
    {
        public int ReferenceID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class MstFOReferenceUpsertRequest
    {
        public int ReferenceID { get; set; }

        [Required(ErrorMessage = "Reference name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ─── COMPLAINT ──────────────────────────────────────────────
    public class FOComplaintViewModel
    {
        public int ComplaintID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ComplaintTypeID { get; set; }
        public int SourceID { get; set; }
        
        public string ComplaintTypeName { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        
        public string ComplaintBy { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime ComplaintDate { get; set; }
        
        public string? Description { get; set; }
        public string? ActionTaken { get; set; }
        public string? AssignedTo { get; set; }
        public string? Note { get; set; }
        public string? Attachment { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class FOComplaintUpsertRequest
    {
        public int ComplaintID { get; set; }

        [Required(ErrorMessage = "Complaint Type is required")]
        public int ComplaintTypeID { get; set; }

        [Required(ErrorMessage = "Source is required")]
        public int SourceID { get; set; }

        [Required(ErrorMessage = "Complaint By is required")]
        [StringLength(150)]
        public string ComplaintBy { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Complaint Date is required")]
        public DateTime ComplaintDate { get; set; }

        public string? Description { get; set; }
        public string? ActionTaken { get; set; }
        
        [StringLength(100)]
        public string? AssignedTo { get; set; }
        
        public string? Note { get; set; }
        
        [StringLength(500)]
        public string? Attachment { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    // ─── POSTAL RECEIVE ────────────────────────────────────────
    public class FOPostalReceiveViewModel
    {
        public int PostalReceiveID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string FromTitle { get; set; } = string.Empty;
        public string? ToTitle { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }
        public byte[]? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class FOPostalReceiveUpsertRequest
    {
        public int PostalReceiveID { get; set; }

        [Required(ErrorMessage = "From Title is required")]
        [StringLength(200)]
        public string FromTitle { get; set; } = string.Empty;

        [StringLength(200)]
        public string? ToTitle { get; set; }

        [StringLength(100)]
        public string? ReferenceNo { get; set; }

        public string? Address { get; set; }
        public string? Note { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        public byte[]? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ─── POSTAL DISPATCH ───────────────────────────────────────
    public class FOPostalDispatchViewModel
    {
        public int PostalDispatchID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string ToTitle { get; set; } = string.Empty;
        public string? FromTitle { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }
        public byte[]? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class FOPostalDispatchUpsertRequest
    {
        public int PostalDispatchID { get; set; }

        [Required(ErrorMessage = "To Title is required")]
        [StringLength(200)]
        public string ToTitle { get; set; } = string.Empty;

        [StringLength(200)]
        public string? FromTitle { get; set; }

        [StringLength(100)]
        public string? ReferenceNo { get; set; }

        public string? Address { get; set; }
        public string? Note { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        public byte[]? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ─── PHONE CALL LOG ───────────────────────────────────────
    public class FOPhoneCallLogViewModel
    {
        public int PhoneCallLogID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public string? CallDuration { get; set; }
        public string? Note { get; set; }
        public string? CallType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class FOPhoneCallLogUpsertRequest
    {
        public int PhoneCallLogID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        public string? Description { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public string? CallDuration { get; set; }
        public string? Note { get; set; }
        public string? CallType { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ─── PAGE VIEW MODEL ────────────────────────────────────────
    public class FrontOfficeSetupPageViewModel
    {
        public List<MstFOPurposeViewModel> Purposes { get; set; } = new();
        public List<MstFOComplaintTypeViewModel> ComplaintTypes { get; set; } = new();
        public List<MstFOSourceViewModel> Sources { get; set; } = new();
        public List<MstFOReferenceViewModel> References { get; set; } = new();
    }

    public class FOComplaintPageViewModel
    {
        public List<FOComplaintViewModel> Complaints { get; set; } = new();
        public List<MstFOComplaintTypeViewModel> ComplaintTypes { get; set; } = new();
        public List<MstFOSourceViewModel> Sources { get; set; } = new();
    }

    public class FOPostalReceivePageViewModel
    {
        public List<FOPostalReceiveViewModel> Items { get; set; } = new();
    }

    public class FOPostalDispatchPageViewModel
    {
        public List<FOPostalDispatchViewModel> Items { get; set; } = new();
    }

    public class FOPhoneCallLogPageViewModel
    {
        public List<FOPhoneCallLogViewModel> Items { get; set; } = new();
    }
}
