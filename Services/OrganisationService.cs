using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This class provides business logic and data access services for OrganisationService.
    /// </summary>
    public class OrganisationService : IOrganisationService
    {
        private readonly SqlHelper _sqlHelper;

        public OrganisationService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<OrganisationViewModel> GetAllOrganisations(bool includeDeleted = false)
        {
            var list = new List<OrganisationViewModel>();
            var parameters = new[] { new SqlParameter("@IncludeDeleted", includeDeleted) };
            var dt = _sqlHelper.ExecuteQuery("sp_Organisations_GetAll", parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        public OrganisationViewModel? GetOrganisationByID(int organisationID)
        {
            var parameters = new[] { new SqlParameter("@OrganisationID", organisationID) };
            var dt = _sqlHelper.ExecuteQuery("sp_Organisations_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        /// <summary>
        /// Translates a massive campus payload into SQL parameters and executes 'sp_Organisations_Upsert'.
        /// Expected to handle everything from financial year mapping constraints to SMS gateway endpoints.
        /// </summary>
        public (bool success, string message) UpsertOrganisation(OrganisationUpsertRequest request, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@OrganisationID", request.OrganisationID),
                    new SqlParameter("@ParentOrganisationID", (object?)request.ParentOrganisationID ?? DBNull.Value),
                    new SqlParameter("@OrganisationName", request.OrganisationName),
                    new SqlParameter("@CompanyCode", request.CompanyCode),
                    new SqlParameter("@FromCode", (object?)request.FromCode ?? DBNull.Value),
                    new SqlParameter("@ToCode", (object?)request.ToCode ?? DBNull.Value),
                    new SqlParameter("@FinancialYear", (object?)request.FinancialYear ?? DBNull.Value),
                    new SqlParameter("@PreviousFinancialYear", (object?)request.PreviousFinancialYear ?? DBNull.Value),
                    new SqlParameter("@CollegeCode", (object?)request.CollegeCode ?? DBNull.Value),
                    new SqlParameter("@Address1", (object?)request.Address1 ?? DBNull.Value),
                    new SqlParameter("@Address2", (object?)request.Address2 ?? DBNull.Value),
                    new SqlParameter("@City", (object?)request.City ?? DBNull.Value),
                    new SqlParameter("@State", (object?)request.State ?? DBNull.Value),
                    new SqlParameter("@Mobile", (object?)request.Mobile ?? DBNull.Value),
                    new SqlParameter("@Phone", (object?)request.Phone ?? DBNull.Value),
                    new SqlParameter("@Email", (object?)request.Email ?? DBNull.Value),
                    new SqlParameter("@Website", (object?)request.Website ?? DBNull.Value),
                    new SqlParameter("@Fax", (object?)request.Fax ?? DBNull.Value),
                    new SqlParameter("@AffiliationNo", (object?)request.AffiliationNo ?? DBNull.Value),
                    new SqlParameter("@BoardName", (object?)request.BoardName ?? DBNull.Value),
                    new SqlParameter("@SchoolStartDate", (object?)request.SchoolStartDate ?? DBNull.Value),
                    new SqlParameter("@RenewalUptoDate", (object?)request.RenewalUptoDate ?? DBNull.Value),
                    new SqlParameter("@SenderID", (object?)request.SenderID ?? DBNull.Value),
                    new SqlParameter("@SMSApiKey", (object?)request.SMSApiKey ?? DBNull.Value),
                    new SqlParameter("@SMSLabel", (object?)request.SMSLabel ?? DBNull.Value),
                    new SqlParameter("@UploadURL", (object?)request.UploadURL ?? DBNull.Value),
                    new SqlParameter("@SessionID", (object?)request.SessionID ?? DBNull.Value),
                    new SqlParameter("@LateFinePerDay", (object?)request.LateFinePerDay ?? DBNull.Value),
                    new SqlParameter("@LibraryFinePerDay", (object?)request.LibraryFinePerDay ?? DBNull.Value),
                    new SqlParameter("@LibraryFineMaxPerBook", (object?)request.LibraryFineMaxPerBook ?? DBNull.Value),
                    new SqlParameter("@FormSalePrice", (object?)request.FormSalePrice ?? DBNull.Value),
                    new SqlParameter("@PunchBeforeMinute", (object?)request.PunchBeforeMinute ?? DBNull.Value),
                    new SqlParameter("@PunchAfterMinute", (object?)request.PunchAfterMinute ?? DBNull.Value),
                    new SqlParameter("@IsFeePayAllowed", request.IsFeePayAllowed),
                    new SqlParameter("@IsChequeAllowed", request.IsChequeAllowed),
                    new SqlParameter("@IsFinalApprovalByHOD", request.IsFinalApprovalByHOD),
                    new SqlParameter("@IsReceiptSearchAll", request.IsReceiptSearchAll),
                    new SqlParameter("@IsScholarshipEnabled", request.IsScholarshipEnabled),
                    new SqlParameter("@IsAttendanceOnline", request.IsAttendanceOnline),
                    new SqlParameter("@IsGroupEnabled", request.IsGroupEnabled),
                    new SqlParameter("@IsLeaveApplyBackDays", request.IsLeaveApplyBackDays),
                    new SqlParameter("@IsPartialFeeAllowed", request.IsPartialFeeAllowed),
                    new SqlParameter("@IsCourseSemesterTemplate", request.IsCourseSemesterTemplate),
                    new SqlParameter("@IsLeaveDefaultApproved", request.IsLeaveDefaultApproved),
                    new SqlParameter("@IsFeeTemplateEnabled", request.IsFeeTemplateEnabled),
                    new SqlParameter("@IsMultiReceiptCopy", request.IsMultiReceiptCopy),
                    new SqlParameter("@IsMultiLanguage", request.IsMultiLanguage),
                    new SqlParameter("@IsSubstituteLeaveApproval", request.IsSubstituteLeaveApproval),
                    new SqlParameter("@IsHostelFeeTemplate", request.IsHostelFeeTemplate),
                    new SqlParameter("@IsAllowSessionChange", request.IsAllowSessionChange),
                    new SqlParameter("@IsOTPLogin", request.IsOTPLogin),
                    new SqlParameter("@IsCopyrightActive", request.IsCopyrightActive),
                    new SqlParameter("@EnquiryMobileNo", (object?)request.EnquiryMobileNo ?? DBNull.Value),
                    new SqlParameter("@EnquiryEmail", (object?)request.EnquiryEmail ?? DBNull.Value),
                    new SqlParameter("@EnquiryWebsite", (object?)request.EnquiryWebsite ?? DBNull.Value),
                    new SqlParameter("@CopyrightText", (object?)request.CopyrightText ?? DBNull.Value),
                    new SqlParameter("@RunBy", (object?)request.RunBy ?? DBNull.Value),
                    new SqlParameter("@PortalUserTypeID", (object?)request.PortalUserTypeID ?? DBNull.Value),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Organisations_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) DeleteOrganisation(int organisationID, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@OrganisationID", organisationID),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Organisations_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) ToggleOrganisationStatus(int organisationID, bool isActive, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@OrganisationID", organisationID),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Organisations_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private OrganisationViewModel MapRowToViewModel(DataRow row)
        {
            return new OrganisationViewModel
            {
                OrganisationID = Convert.ToInt32(row["OrganisationID"]),
                ParentOrganisationID = row["ParentOrganisationID"] != DBNull.Value ? Convert.ToInt32(row["ParentOrganisationID"]) : null,
                ParentName = row.Table.Columns.Contains("ParentName") && row["ParentName"] != DBNull.Value ? row["ParentName"].ToString() : null,
                OrganisationName = row["OrganisationName"].ToString() ?? "",
                CompanyCode = row["CompanyCode"].ToString() ?? "",
                FromCode = row["FromCode"] != DBNull.Value ? Convert.ToInt32(row["FromCode"]) : null,
                ToCode = row["ToCode"] != DBNull.Value ? Convert.ToInt32(row["ToCode"]) : null,
                FinancialYear = row["FinancialYear"]?.ToString(),
                PreviousFinancialYear = row["PreviousFinancialYear"] != DBNull.Value ? Convert.ToInt32(row["PreviousFinancialYear"]) : null,
                CollegeCode = row["CollegeCode"]?.ToString(),
                Address1 = row["Address1"]?.ToString(),
                Address2 = row["Address2"]?.ToString(),
                City = row["City"]?.ToString(),
                State = row["State"]?.ToString(),
                Mobile = row["Mobile"]?.ToString(),
                Phone = row["Phone"]?.ToString(),
                Email = row["Email"]?.ToString(),
                Website = row["Website"]?.ToString(),
                Fax = row["Fax"]?.ToString(),
                AffiliationNo = row["AffiliationNo"]?.ToString(),
                BoardName = row["BoardName"]?.ToString(),
                SchoolStartDate = row["SchoolStartDate"] != DBNull.Value ? Convert.ToDateTime(row["SchoolStartDate"]) : null,
                RenewalUptoDate = row["RenewalUptoDate"] != DBNull.Value ? Convert.ToDateTime(row["RenewalUptoDate"]) : null,
                SenderID = row["SenderID"]?.ToString(),
                SMSApiKey = row["SMSApiKey"]?.ToString(),
                SMSLabel = row["SMSLabel"]?.ToString(),
                UploadURL = row["UploadURL"]?.ToString(),
                SessionID = row["SessionID"] != DBNull.Value ? Convert.ToInt32(row["SessionID"]) : null,
                LateFinePerDay = row["LateFinePerDay"] != DBNull.Value ? Convert.ToDecimal(row["LateFinePerDay"]) : null,
                LibraryFinePerDay = row["LibraryFinePerDay"] != DBNull.Value ? Convert.ToDecimal(row["LibraryFinePerDay"]) : null,
                LibraryFineMaxPerBook = row["LibraryFineMaxPerBook"] != DBNull.Value ? Convert.ToDecimal(row["LibraryFineMaxPerBook"]) : null,
                FormSalePrice = row["FormSalePrice"] != DBNull.Value ? Convert.ToDecimal(row["FormSalePrice"]) : null,
                PunchBeforeMinute = row["PunchBeforeMinute"] != DBNull.Value ? Convert.ToInt32(row["PunchBeforeMinute"]) : null,
                PunchAfterMinute = row["PunchAfterMinute"] != DBNull.Value ? Convert.ToInt32(row["PunchAfterMinute"]) : null,
                IsFeePayAllowed = Convert.ToBoolean(row["IsFeePayAllowed"]),
                IsChequeAllowed = Convert.ToBoolean(row["IsChequeAllowed"]),
                IsFinalApprovalByHOD = Convert.ToBoolean(row["IsFinalApprovalByHOD"]),
                IsReceiptSearchAll = Convert.ToBoolean(row["IsReceiptSearchAll"]),
                IsScholarshipEnabled = Convert.ToBoolean(row["IsScholarshipEnabled"]),
                IsAttendanceOnline = Convert.ToBoolean(row["IsAttendanceOnline"]),
                IsGroupEnabled = Convert.ToBoolean(row["IsGroupEnabled"]),
                IsLeaveApplyBackDays = Convert.ToBoolean(row["IsLeaveApplyBackDays"]),
                IsPartialFeeAllowed = Convert.ToBoolean(row["IsPartialFeeAllowed"]),
                IsCourseSemesterTemplate = Convert.ToBoolean(row["IsCourseSemesterTemplate"]),
                IsLeaveDefaultApproved = Convert.ToBoolean(row["IsLeaveDefaultApproved"]),
                IsFeeTemplateEnabled = Convert.ToBoolean(row["IsFeeTemplateEnabled"]),
                IsMultiReceiptCopy = Convert.ToBoolean(row["IsMultiReceiptCopy"]),
                IsMultiLanguage = Convert.ToBoolean(row["IsMultiLanguage"]),
                IsSubstituteLeaveApproval = Convert.ToBoolean(row["IsSubstituteLeaveApproval"]),
                IsHostelFeeTemplate = Convert.ToBoolean(row["IsHostelFeeTemplate"]),
                IsAllowSessionChange = Convert.ToBoolean(row["IsAllowSessionChange"]),
                IsOTPLogin = Convert.ToBoolean(row["IsOTPLogin"]),
                IsCopyrightActive = Convert.ToBoolean(row["IsCopyrightActive"]),
                EnquiryMobileNo = row["EnquiryMobileNo"]?.ToString(),
                EnquiryEmail = row["EnquiryEmail"]?.ToString(),
                EnquiryWebsite = row["EnquiryWebsite"]?.ToString(),
                CopyrightText = row["CopyrightText"]?.ToString(),
                RunBy = row["RunBy"]?.ToString(),
                PortalUserTypeID = row["PortalUserTypeID"] != DBNull.Value ? Convert.ToInt32(row["PortalUserTypeID"]) : null,
                CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                IsActive = Convert.ToBoolean(row["IsActive"])
            };
        }
    }
}
