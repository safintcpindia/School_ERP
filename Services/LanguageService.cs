using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This class provides business logic and data access services for LanguageService.
    /// </summary>
    public class LanguageService : ILanguageService
    {
        private readonly SqlHelper _sqlHelper;

        public LanguageService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<MstLanguageViewModel> GetAllLanguages(bool includeDeleted = false)
        {
            var list = new List<MstLanguageViewModel>();
            var parameters = new[] { new SqlParameter("@IncludeDeleted", includeDeleted) };
            var dt = _sqlHelper.ExecuteQuery("sp_Languages_GetAll", parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        public MstLanguageViewModel? GetLanguageByID(int languageId)
        {
            var parameters = new[] { new SqlParameter("@LanguageId", languageId) };
            var dt = _sqlHelper.ExecuteQuery("sp_Languages_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        /// <summary>
        /// Directly invokes 'sp_Languages_Upsert' to inject a new ISO locale definition and its RTL flags.
        /// </summary>
        public (bool success, string message) UpsertLanguage(MstLanguageUpsertRequest request, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@LanguageId", request.LanguageId),
                    new SqlParameter("@LanguageTitle", request.LanguageTitle),
                    new SqlParameter("@LanguageCode", request.LanguageCode),
                    new SqlParameter("@LanguageIcon", (object)request.LanguageIcon ?? DBNull.Value),
                    new SqlParameter("@LanguageIsRtl", request.LanguageIsRtl),
                    new SqlParameter("@LanguageBase", request.LanguageBase),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Languages_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) DeleteLanguage(int languageId, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@LanguageId", languageId),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Languages_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) ToggleLanguageStatus(int languageId, bool isActive, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@LanguageId", languageId),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Languages_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private MstLanguageViewModel MapRowToViewModel(DataRow row)
        {
            return new MstLanguageViewModel
            {
                LanguageId = Convert.ToInt32(row["LanguageId"]),
                LanguageTitle = row["LanguageTitle"].ToString() ?? "",
                LanguageCode = row["LanguageCode"].ToString() ?? "",
                LanguageIcon = row["LanguageIcon"].ToString() ?? "",
                LanguageIsRtl = Convert.ToBoolean(row["LanguageIsRtl"]),
                LanguageBase = Convert.ToBoolean(row["LanguageBase"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };
        }
    }
}
