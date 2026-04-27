using System;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This service handles the actual work of managing SMS configuration, including saving and retrieving gateway settings from the database.
    /// </summary>
    public class SmsConfigService : ISmsConfigService
    {
        private readonly SqlHelper _sqlHelper;

        public SmsConfigService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        /// <summary>
        /// Retrieves the current SMS gateway settings from the database.
        /// </summary>
        public MstSmsConfigViewModel? GetSmsConfig()
        {
            var dt = _sqlHelper.ExecuteQuery("sp_SmsConfig_Get", null!);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates SMS gateway settings (like the gateway name, API link, and secret key) in the database.
        /// </summary>
        public (bool success, string message) UpsertSmsConfig(MstSmsConfigUpsertRequest request, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@SmsId", request.SmsId),
                    new SqlParameter("@SmsGateway", (object)request.SmsGateway ?? DBNull.Value),
                    new SqlParameter("@SmsStatus", (object)request.SmsStatus ?? DBNull.Value),
                    new SqlParameter("@SmsApiUrl", (object)request.SmsApiUrl ?? DBNull.Value),
                    new SqlParameter("@SmsApiKey", (object)request.SmsApiKey ?? DBNull.Value),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_SmsConfig_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// A helper tool that converts raw database information about SMS settings into a format the application can use.
        /// </summary>
        private MstSmsConfigViewModel MapRowToViewModel(DataRow row)
        {
            return new MstSmsConfigViewModel
            {
                SmsId = Convert.ToInt32(row["SmsId"]),
                SmsGateway = row["SmsGateway"].ToString() ?? "",
                SmsStatus = row["SmsStatus"].ToString() ?? "",
                SmsApiUrl = row["SmsApiUrl"].ToString() ?? "",
                SmsApiKey = row["SmsApiKey"].ToString() ?? "",
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]) : (DateTime?)null,
                ModifiedOn = row["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedOn"]) : (DateTime?)null
            };
        }
    }
}
