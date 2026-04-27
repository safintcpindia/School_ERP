using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This service handles the actual work of managing language settings, such as saving new languages and updating their details in the database.
    /// </summary>
    public class LanguageService : ILanguageService
    {
        private readonly SqlHelper _sqlHelper;

        public LanguageService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        /// <summary>
        /// Retrieves a list of all languages from the database.
        /// </summary>
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

        /// <summary>
        /// Looks up the details of a specific language using its unique ID.
        /// </summary>
        public MstLanguageViewModel? GetLanguageByID(int languageId)
        {
            var parameters = new[] { new SqlParameter("@LanguageId", languageId) };
            var dt = _sqlHelper.ExecuteQuery("sp_Languages_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates language information in the database, including the name, code, and whether it is a right-to-left language.
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

        /// <summary>
        /// Deletes a language record from the database.
        /// </summary>
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

        /// <summary>
        /// Updates whether a language is currently enabled for use in the application.
        /// </summary>
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

        /// <summary>
        /// A helper tool that converts raw database data about languages into a format that the application can easily understand.
        /// </summary>
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
