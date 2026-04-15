using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This class provides business logic and data access services for SessionService.
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly SqlHelper _sqlHelper;

        public SessionService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<MstSessionViewModel> GetAllSessions(bool includeDeleted = false)
        {
            var list = new List<MstSessionViewModel>();
            var parameters = new[] { new SqlParameter("@IncludeDeleted", includeDeleted) };
            var dt = _sqlHelper.ExecuteQuery("sp_Sessions_GetAll", parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        public MstSessionViewModel? GetSessionByID(int sessionId)
        {
            var parameters = new[] { new SqlParameter("@SessionId", sessionId) };
            var dt = _sqlHelper.ExecuteQuery("sp_Sessions_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        /// <summary>
        /// Inserts or modifies a chronological filtering boundary by tying it to user auditing signatures.
        /// Returns a boolean mapping of the 'sp_Sessions_Upsert' success tuple.
        /// </summary>
        public (bool success, string message) UpsertSession(MstSessionUpsertRequest request, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@SessionId", request.SessionId),
                    new SqlParameter("@SessionTitle", request.SessionTitle),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Sessions_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) DeleteSession(int sessionId, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@SessionId", sessionId),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Sessions_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) ToggleSessionStatus(int sessionId, bool isActive, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@SessionId", sessionId),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Sessions_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private MstSessionViewModel MapRowToViewModel(DataRow row)
        {
            return new MstSessionViewModel
            {
                SessionId = Convert.ToInt32(row["SessionId"]),
                SessionTitle = row["SessionTitle"].ToString() ?? "",
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                ModifiedBy = row["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(row["ModifiedBy"]) : null,
                ModifiedOn = row["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedOn"]) : null
            };
        }
    }
}
