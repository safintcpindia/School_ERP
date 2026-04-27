using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This service handles the actual work of managing academic sessions, such as saving session names and tracking which session a user is currently logged into in the database.
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly SqlHelper _sqlHelper;

        public SessionService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        /// <summary>
        /// Retrieves a list of all academic sessions from the database.
        /// </summary>
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

        /// <summary>
        /// Looks up the details of a single session using its ID.
        /// </summary>
        public MstSessionViewModel? GetSessionByID(int sessionId)
        {
            var parameters = new[] { new SqlParameter("@SessionId", sessionId) };
            var dt = _sqlHelper.ExecuteQuery("sp_Sessions_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates academic session information in the database.
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

        /// <summary>
        /// Deletes a session record from the database.
        /// </summary>
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

        /// <summary>
        /// Updates whether a session is currently available for use.
        /// </summary>
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

        /// <summary>
        /// Saves the user's current session choice in the database so they stay in the same session even after refreshing.
        /// </summary>
        public (bool success, string message) UpdateUserCurrentSession(int userId, int sessionId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@DoneBy", userId),
                    new SqlParameter("@CurrentSessionId", sessionId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Users_UpdateCurrentSession", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Retrieves the user's currently selected academic session from the database.
        /// </summary>
        public int? GetUserCurrentSession(int userId)
        {
            try
            {
                var parameters = new[] { new SqlParameter("@UserID", userId) };
                var dt = _sqlHelper.ExecuteQuery("sp_Users_GetCurrentSession", parameters);
                if (dt.Rows.Count == 0 || dt.Rows[0]["SessionId"] == DBNull.Value) return null;
                return Convert.ToInt32(dt.Rows[0]["SessionId"]);
            }
            catch { return null; }
        }

        /// <summary>
        /// A helper tool that converts raw database data about sessions into a format the application can easily work with.
        /// </summary>
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
