using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class StudentCertificateService : IStudentCertificateService
    {
        private readonly SqlHelper _sqlHelper;

        public StudentCertificateService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<StudentCertificateViewModel> GetAll(int companyId, int sessionId)
        {
            var list = new List<StudentCertificateViewModel>();
            var p = new[]
            {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_StudentCertificate_GetAll", p);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new StudentCertificateViewModel
                {
                    CertificateID = Convert.ToInt32(row["CertificateID"]),
                    CertificateName = row["CertificateName"].ToString(),
                    HeaderLeftText = row["HeaderLeftText"]?.ToString(),
                    HeaderCenterText = row["HeaderCenterText"]?.ToString(),
                    HeaderRightText = row["HeaderRightText"]?.ToString(),
                    FooterLeftText = row["FooterLeftText"]?.ToString(),
                    FooterCenterText = row["FooterCenterText"]?.ToString(),
                    FooterRightText = row["FooterRightText"]?.ToString(),
                    EnableStudentPhoto = Convert.ToBoolean(row["EnableStudentPhoto"]),
                    BackgroundImage = row["BackgroundImage"] != DBNull.Value ? (byte[])row["BackgroundImage"] : null,
                    BackgroundImageName = row["BackgroundImageName"]?.ToString(),
                    BackgroundImageType = row["BackgroundImageType"]?.ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedOn = Convert.ToDateTime(row["CreatedOn"])
                });
            }
            return list;
        }

        public StudentCertificateViewModel GetByID(int id)
        {
            var p = new[] { new SqlParameter("@CertificateID", id) };
            var dt = _sqlHelper.ExecuteQuery("sp_StudentCertificate_GetByID", p);
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new StudentCertificateViewModel
            {
                CertificateID = Convert.ToInt32(row["CertificateID"]),
                CertificateName = row["CertificateName"].ToString(),
                HeaderLeftText = row["HeaderLeftText"]?.ToString(),
                HeaderCenterText = row["HeaderCenterText"]?.ToString(),
                HeaderRightText = row["HeaderRightText"]?.ToString(),
                BodyText = row["BodyText"]?.ToString(),
                FooterLeftText = row["FooterLeftText"]?.ToString(),
                FooterCenterText = row["FooterCenterText"]?.ToString(),
                FooterRightText = row["FooterRightText"]?.ToString(),
                HeaderHeight = row["HeaderHeight"] != DBNull.Value ? Convert.ToDecimal(row["HeaderHeight"]) : null,
                FooterHeight = row["FooterHeight"] != DBNull.Value ? Convert.ToDecimal(row["FooterHeight"]) : null,
                BodyHeight = row["BodyHeight"] != DBNull.Value ? Convert.ToDecimal(row["BodyHeight"]) : null,
                BodyWidth = row["BodyWidth"] != DBNull.Value ? Convert.ToDecimal(row["BodyWidth"]) : null,
                EnableStudentPhoto = Convert.ToBoolean(row["EnableStudentPhoto"]),
                BackgroundImage = row["BackgroundImage"] != DBNull.Value ? (byte[])row["BackgroundImage"] : null,
                BackgroundImageType = row["BackgroundImageType"]?.ToString(),
                BackgroundImageName = row["BackgroundImageName"]?.ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };
        }

        public (int Result, string Message) Upsert(StudentCertificateUpsertRequest request, int userId, int companyId, int sessionId)
        {
            if (request.CertificateID > 0 && (request.BackgroundImage == null || request.BackgroundImage.Length == 0))
            {
                var existing = GetByID(request.CertificateID);
                if (existing != null && existing.BackgroundImage != null)
                {
                    request.BackgroundImage = existing.BackgroundImage;
                    request.BackgroundImageType = existing.BackgroundImageType;
                    request.BackgroundImageName = existing.BackgroundImageName;
                }
            }

            var p = new[]
            {
                new SqlParameter("@CertificateID", request.CertificateID),
                new SqlParameter("@CertificateName", request.CertificateName),
                new SqlParameter("@HeaderLeftText", (object)request.HeaderLeftText ?? DBNull.Value),
                new SqlParameter("@HeaderCenterText", (object)request.HeaderCenterText ?? DBNull.Value),
                new SqlParameter("@HeaderRightText", (object)request.HeaderRightText ?? DBNull.Value),
                new SqlParameter("@BodyText", request.BodyText),
                new SqlParameter("@FooterLeftText", (object)request.FooterLeftText ?? DBNull.Value),
                new SqlParameter("@FooterCenterText", (object)request.FooterCenterText ?? DBNull.Value),
                new SqlParameter("@FooterRightText", (object)request.FooterRightText ?? DBNull.Value),
                new SqlParameter("@HeaderHeight", (object)request.HeaderHeight ?? DBNull.Value),
                new SqlParameter("@FooterHeight", (object)request.FooterHeight ?? DBNull.Value),
                new SqlParameter("@BodyHeight", (object)request.BodyHeight ?? DBNull.Value),
                new SqlParameter("@BodyWidth", (object)request.BodyWidth ?? DBNull.Value),
                new SqlParameter("@EnableStudentPhoto", request.EnableStudentPhoto),
                new SqlParameter("@BackgroundImage", (object)request.BackgroundImage ?? DBNull.Value),
                new SqlParameter("@BackgroundImageType", (object)request.BackgroundImageType ?? DBNull.Value),
                new SqlParameter("@BackgroundImageName", (object)request.BackgroundImageName ?? DBNull.Value),
                new SqlParameter("@IsActive", request.IsActive),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_StudentCertificate_Upsert", p);
            if (dt != null && dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["Result"]), dt.Rows[0]["Message"]?.ToString() ?? "");
            }
            return (0, "Failed to upsert certificate template.");
        }

        public (int Result, string Message) Delete(int id, int userId)
        {
            var p = new[]
            {
                new SqlParameter("@CertificateID", id),
                new SqlParameter("@UserId", userId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_StudentCertificate_Delete", p);
            if (dt != null && dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["Result"]), dt.Rows[0]["Message"]?.ToString() ?? "");
            }
            return (0, "Failed to delete certificate template.");
        }

        public (int Result, string Message) ToggleStatus(int id, bool isActive, int userId)
        {
            var p = new[]
            {
                new SqlParameter("@CertificateID", id),
                new SqlParameter("@IsActive", isActive),
                new SqlParameter("@UserId", userId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_StudentCertificate_ToggleStatus", p);
            if (dt != null && dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["Result"]), dt.Rows[0]["Message"]?.ToString() ?? "");
            }
            return (0, "Failed to toggle status.");
        }

        public string GenerateCertificate(int studentId, int certificateId, int companyId, int sessionId)
        {
            var template = GetByID(certificateId);
            if (template == null) return "Template not found";

            var p = new[] { 
                new SqlParameter("@STUDENTID", studentId),
                new SqlParameter("@COMPANYID", companyId),
                new SqlParameter("@SESSIONID", sessionId)
            };
            var ds = _sqlHelper.ExecuteDataSet("SP_STUDENT_DETAILS_GET", p);
            
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return "Student not found";
            var row = ds.Tables[0].Rows[0];

            string body = template.BodyText ?? "";
            
            // Helper function to safely get column value
            string GetVal(string col) {
                if (!row.Table.Columns.Contains(col)) {
                    // Try case-insensitive search
                    foreach (DataColumn c in row.Table.Columns) {
                        if (string.Equals(c.ColumnName, col, StringComparison.OrdinalIgnoreCase)) return row[c.ColumnName]?.ToString() ?? "";
                    }
                    return "";
                }
                return row[col]?.ToString() ?? "";
            }

            // Map placeholders
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "[name]", (GetVal("FIRSTNAME") + " " + GetVal("MIDDLENAME") + " " + GetVal("LASTNAME")).Trim().Replace("  ", " ") },
                { "[dob]", row.Table.Columns.Contains("DOB") && row["DOB"] != DBNull.Value ? Convert.ToDateTime(row["DOB"]).ToString("dd-MM-yyyy") : "" },
                { "[present_address]", GetVal("CURRENTADDRESS") },
                { "[guardian]", GetVal("GUARDIANNAME") },
                { "[admission_no]", GetVal("ADMISSIONNO") },
                { "[roll_no]", GetVal("ROLLNO") },
                { "[class]", GetVal("ClassName") },
                { "[section]", GetVal("SectionName") },
                { "[gender]", GetVal("GENDER") },
                { "[admission_date]", row.Table.Columns.Contains("ADMISSIONDATE") && row["ADMISSIONDATE"] != DBNull.Value ? Convert.ToDateTime(row["ADMISSIONDATE"]).ToString("dd-MM-yyyy") : "" },
                { "[father_name]", GetVal("FATHERNAME") },
                { "[mother_name]", GetVal("MOTHERNAME") },
                { "[phone]", GetVal("MOBILENO") },
                { "[email]", GetVal("EMAIL") },
                { "[religion]", GetVal("RELIGION") },
                { "[category]", GetVal("StudentCategoryName") },
                { "[cast]", GetVal("CASTE") },
                { "[created_at]", DateTime.Now.ToString("dd-MM-yyyy") }
            };

            // Handle Student Photo
            string studentPhotoHtml = "";
            if (template.EnableStudentPhoto)
            {
                byte[] photo = row.Table.Columns.Contains("STUDENTPHOTO") && row["STUDENTPHOTO"] != DBNull.Value ? (byte[])row["STUDENTPHOTO"] : null;
                string photoType = row.Table.Columns.Contains("STUDENTPHOTOTYPE") && row["STUDENTPHOTOTYPE"] != DBNull.Value ? row["STUDENTPHOTOTYPE"].ToString() : "image/png";

                if (photo != null && photo.Length > 0)
                {
                    string base64 = Convert.ToBase64String(photo);
                    studentPhotoHtml = $"<img src=\"data:{photoType};base64,{base64}\" style=\"width:100px; height:auto; border:1px solid #ccc;\" alt=\"Student Photo\" />";
                }
            }
            body = body.Replace("[student_photo]", studentPhotoHtml, StringComparison.OrdinalIgnoreCase);

            foreach (var item in map)
            {
                body = body.Replace(item.Key, item.Value, StringComparison.OrdinalIgnoreCase);
            }

            return body;
        }
    }
}
