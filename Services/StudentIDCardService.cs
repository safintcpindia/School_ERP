using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class StudentIDCardService : IStudentIDCardService
    {
        private readonly SqlHelper _sqlHelper;

        public StudentIDCardService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<StudentIDCardViewModel> GetAll(int companyId, int sessionId)
        {
            var list = new List<StudentIDCardViewModel>();
            var p = new[]
            {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_StudentIDCard_GetAll", p);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        public StudentIDCardViewModel GetByID(int id)
        {
            var p = new[] { new SqlParameter("@IDCardID", id) };
            var dt = _sqlHelper.ExecuteQuery("sp_StudentIDCard_GetByID", p);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        public (int Result, string Message) Upsert(StudentIDCardUpsertRequest request, int userId, int companyId, int sessionId)
        {
            // Image Preservation Logic
            if (request.IDCardID > 0)
            {
                var existing = GetByID(request.IDCardID);
                if (existing != null)
                {
                    if (request.BackgroundImage == null || request.BackgroundImage.Length == 0)
                    {
                        request.BackgroundImage = existing.BackgroundImage;
                        request.BackgroundImageType = existing.BackgroundImageType;
                        request.BackgroundImageName = existing.BackgroundImageName;
                    }
                    if (request.LogoImage == null || request.LogoImage.Length == 0)
                    {
                        request.LogoImage = existing.LogoImage;
                        request.LogoImageType = existing.LogoImageType;
                        request.LogoImageName = existing.LogoImageName;
                    }
                    if (request.SignatureImage == null || request.SignatureImage.Length == 0)
                    {
                        request.SignatureImage = existing.SignatureImage;
                        request.SignatureImageType = existing.SignatureImageType;
                        request.SignatureImageName = existing.SignatureImageName;
                    }
                }
            }

            var p = new[]
            {
                new SqlParameter("@IDCardID", request.IDCardID),
                new SqlParameter("@IDCardTitle", request.IDCardTitle),
                new SqlParameter("@SchoolName", (object)request.SchoolName ?? DBNull.Value),
                new SqlParameter("@HeaderColor", (object)request.HeaderColor ?? DBNull.Value),
                new SqlParameter("@AddressPhoneEmail", (object)request.AddressPhoneEmail ?? DBNull.Value),
                
                new SqlParameter("@BackgroundImage", (object)request.BackgroundImage ?? DBNull.Value),
                new SqlParameter("@BackgroundImageType", (object)request.BackgroundImageType ?? DBNull.Value),
                new SqlParameter("@BackgroundImageName", (object)request.BackgroundImageName ?? DBNull.Value),
                
                new SqlParameter("@LogoImage", (object)request.LogoImage ?? DBNull.Value),
                new SqlParameter("@LogoImageType", (object)request.LogoImageType ?? DBNull.Value),
                new SqlParameter("@LogoImageName", (object)request.LogoImageName ?? DBNull.Value),
                
                new SqlParameter("@SignatureImage", (object)request.SignatureImage ?? DBNull.Value),
                new SqlParameter("@SignatureImageType", (object)request.SignatureImageType ?? DBNull.Value),
                new SqlParameter("@SignatureImageName", (object)request.SignatureImageName ?? DBNull.Value),
                
                new SqlParameter("@ShowAdmissionNo", request.ShowAdmissionNo),
                new SqlParameter("@ShowStudentName", request.ShowStudentName),
                new SqlParameter("@ShowClass", request.ShowClass),
                new SqlParameter("@ShowFatherName", request.ShowFatherName),
                new SqlParameter("@ShowMotherName", request.ShowMotherName),
                new SqlParameter("@ShowStudentAddress", request.ShowStudentAddress),
                new SqlParameter("@ShowPhone", request.ShowPhone),
                new SqlParameter("@ShowDOB", request.ShowDOB),
                new SqlParameter("@ShowBloodGroup", request.ShowBloodGroup),
                new SqlParameter("@DesignType", request.DesignType),
                new SqlParameter("@ShowBarcode", request.ShowBarcode),
                
                new SqlParameter("@IsActive", request.IsActive),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_StudentIDCard_Upsert", p);
            if (dt != null && dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["Result"]), dt.Rows[0]["Message"]?.ToString() ?? "");
            }
            return (0, "Failed to upsert ID Card template.");
        }

        public (int Result, string Message) Delete(int id, int userId)
        {
            var p = new[]
            {
                new SqlParameter("@IDCardID", id),
                new SqlParameter("@UserId", userId)
            };
            var dt = _sqlHelper.ExecuteQuery("sp_StudentIDCard_Delete", p);
            if (dt != null && dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["Result"]), dt.Rows[0]["Message"]?.ToString() ?? "");
            }
            return (0, "Failed to delete template.");
        }

        public (int Result, string Message) ToggleStatus(int id, bool isActive, int userId)
        {
            var p = new[]
            {
                new SqlParameter("@IDCardID", id),
                new SqlParameter("@IsActive", isActive),
                new SqlParameter("@UserId", userId)
            };
            var dt = _sqlHelper.ExecuteQuery("sp_StudentIDCard_ToggleStatus", p);
            if (dt != null && dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["Result"]), dt.Rows[0]["Message"]?.ToString() ?? "");
            }
            return (0, "Failed to toggle status.");
        }

        public string GenerateIDCard(int studentId, int idCardId, int companyId, int sessionId)
        {
            // Implementation for ID card generation (placeholder replacement or specialized rendering)
            // For now, we'll return the raw template or a structured JSON that the frontend uses.
            return "ID Card Generated"; 
        }

        private StudentIDCardViewModel MapRowToViewModel(DataRow row)
        {
            return new StudentIDCardViewModel
            {
                IDCardID = Convert.ToInt32(row["IDCardID"]),
                IDCardTitle = row["IDCardTitle"].ToString(),
                SchoolName = row["SchoolName"]?.ToString(),
                HeaderColor = row["HeaderColor"]?.ToString(),
                AddressPhoneEmail = row["AddressPhoneEmail"]?.ToString(),
                
                BackgroundImage = row["BackgroundImage"] != DBNull.Value ? (byte[])row["BackgroundImage"] : null,
                BackgroundImageType = row["BackgroundImageType"]?.ToString(),
                BackgroundImageName = row["BackgroundImageName"]?.ToString(),
                
                LogoImage = row["LogoImage"] != DBNull.Value ? (byte[])row["LogoImage"] : null,
                LogoImageType = row["LogoImageType"]?.ToString(),
                LogoImageName = row["LogoImageName"]?.ToString(),
                
                SignatureImage = row["SignatureImage"] != DBNull.Value ? (byte[])row["SignatureImage"] : null,
                SignatureImageType = row["SignatureImageType"]?.ToString(),
                SignatureImageName = row["SignatureImageName"]?.ToString(),
                
                ShowAdmissionNo = Convert.ToBoolean(row["ShowAdmissionNo"]),
                ShowStudentName = Convert.ToBoolean(row["ShowStudentName"]),
                ShowClass = Convert.ToBoolean(row["ShowClass"]),
                ShowFatherName = Convert.ToBoolean(row["ShowFatherName"]),
                ShowMotherName = Convert.ToBoolean(row["ShowMotherName"]),
                ShowStudentAddress = Convert.ToBoolean(row["ShowStudentAddress"]),
                ShowPhone = Convert.ToBoolean(row["ShowPhone"]),
                ShowDOB = Convert.ToBoolean(row["ShowDOB"]),
                ShowBloodGroup = Convert.ToBoolean(row["ShowBloodGroup"]),
                DesignType = Convert.ToInt32(row["DesignType"]),
                ShowBarcode = Convert.ToBoolean(row["ShowBarcode"]),
                
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };
        }
    }
}
