using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class StaffIDCardService : IStaffIDCardService
    {
        private readonly SqlHelper _sqlHelper;

        public StaffIDCardService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<StaffIDCardViewModel> GetAll(int companyId, int sessionId)
        {
            var list = new List<StaffIDCardViewModel>();
            var p = new[]
            {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_StaffIDCard_GetAll", p);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        public StaffIDCardViewModel GetByID(int id)
        {
            var p = new[] { new SqlParameter("@IDCardID", id) };
            var dt = _sqlHelper.ExecuteQuery("sp_StaffIDCard_GetByID", p);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        public (int Result, string Message) Upsert(StaffIDCardUpsertRequest request, int userId, int companyId, int sessionId)
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
                
                new SqlParameter("@ShowStaffName", request.ShowStaffName),
                new SqlParameter("@ShowDesignation", request.ShowDesignation),
                new SqlParameter("@ShowStaffID", request.ShowStaffID),
                new SqlParameter("@ShowDepartment", request.ShowDepartment),
                new SqlParameter("@ShowDOJ", request.ShowDOJ),
                new SqlParameter("@ShowPhone", request.ShowPhone),
                new SqlParameter("@ShowBloodGroup", request.ShowBloodGroup),
                new SqlParameter("@ShowStaffAddress", request.ShowStaffAddress),
                new SqlParameter("@DesignType", request.DesignType),
                new SqlParameter("@ShowBarcode", request.ShowBarcode),
                
                new SqlParameter("@IsActive", request.IsActive),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_StaffIDCard_Upsert", p);
            if (dt != null && dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["Result"]), dt.Rows[0]["Message"]?.ToString() ?? "");
            }
            return (0, "Failed to upsert Staff ID Card template.");
        }

        public (int Result, string Message) Delete(int id, int userId)
        {
            var p = new[]
            {
                new SqlParameter("@IDCardID", id),
                new SqlParameter("@UserId", userId)
            };
            var dt = _sqlHelper.ExecuteQuery("sp_StaffIDCard_Delete", p);
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
            var dt = _sqlHelper.ExecuteQuery("sp_StaffIDCard_ToggleStatus", p);
            if (dt != null && dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["Result"]), dt.Rows[0]["Message"]?.ToString() ?? "");
            }
            return (0, "Failed to toggle status.");
        }

        private StaffIDCardViewModel MapRowToViewModel(DataRow row)
        {
            return new StaffIDCardViewModel
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
                
                ShowStaffName = Convert.ToBoolean(row["ShowStaffName"]),
                ShowDesignation = Convert.ToBoolean(row["ShowDesignation"]),
                ShowStaffID = Convert.ToBoolean(row["ShowStaffID"]),
                ShowDepartment = Convert.ToBoolean(row["ShowDepartment"]),
                ShowDOJ = Convert.ToBoolean(row["ShowDOJ"]),
                ShowPhone = Convert.ToBoolean(row["ShowPhone"]),
                ShowBloodGroup = Convert.ToBoolean(row["ShowBloodGroup"]),
                ShowStaffAddress = Convert.ToBoolean(row["ShowStaffAddress"]),
                DesignType = Convert.ToInt32(row["DesignType"]),
                ShowBarcode = Convert.ToBoolean(row["ShowBarcode"]),
                
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };
        }
    }
}
