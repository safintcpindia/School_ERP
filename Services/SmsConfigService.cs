using System;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This class provides business logic and data access services for SmsConfigService.
    /// </summary>
    public class SmsConfigService : ISmsConfigService
    {
        private readonly SqlHelper _sqlHelper;

        public SmsConfigService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public MstSmsConfigViewModel? GetSmsConfig()
        {
            var dt = _sqlHelper.ExecuteQuery("sp_SmsConfig_Get", null!);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        /// <summary>
        /// Upserts the gateway URL and secret key blocks into the master properties table via sp_SmsConfig_Upsert.
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
