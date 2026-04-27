using System;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This service handles the actual work of managing email configuration, including saving and retrieving server settings from the database.
    /// </summary>
    public class EmailConfigService : IEmailConfigService
    {
        private readonly SqlHelper _sqlHelper;

        public EmailConfigService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        /// <summary>
        /// Retrieves the current email server settings from the database.
        /// </summary>
        public MstEmailConfigViewModel? GetEmailConfig()
        {
            var dt = _sqlHelper.ExecuteQuery("sp_EmailConfig_Get", null!);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates email server settings (like server address, port, and security options) in the database.
        /// </summary>
        public (bool success, string message) UpsertEmailConfig(MstEmailConfigUpsertRequest request, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@EmailId", request.EmailId),
                    new SqlParameter("@EmailType", request.EmailType),
                    new SqlParameter("@SMTPServer", (object)request.SMTPServer ?? DBNull.Value),
                    new SqlParameter("@SMTPPort", (object)request.SMTPPort ?? DBNull.Value),
                    new SqlParameter("@SMTPUsername", (object)request.SMTPUsername ?? DBNull.Value),
                    new SqlParameter("@SMTPPassword", (object)request.SMTPPassword ?? DBNull.Value),
                    new SqlParameter("@SslTls", (object)request.SslTls ?? DBNull.Value),
                    new SqlParameter("@SMTPAuth", (object)request.SMTPAuth ?? DBNull.Value),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_EmailConfig_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// A helper tool that converts raw database information about email settings into a format the application can use.
        /// </summary>
        private MstEmailConfigViewModel MapRowToViewModel(DataRow row)
        {
            return new MstEmailConfigViewModel
            {
                EmailId = Convert.ToInt32(row["EmailId"]),
                EmailType = row["EmailType"].ToString() ?? "",
                SMTPServer = row["SMTPServer"].ToString() ?? "",
                SMTPPort = row["SMTPPort"].ToString() ?? "",
                SMTPUsername = row["SMTPUsername"].ToString() ?? "",
                SMTPPassword = row["SMTPPassword"].ToString() ?? "",
                SslTls = row["SslTls"].ToString() ?? "",
                SMTPAuth = row["SMTPAuth"].ToString() ?? "",
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]) : (DateTime?)null,
                ModifiedOn = row["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedOn"]) : (DateTime?)null
            };
        }
    }
}
