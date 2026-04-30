using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This service handles the actual work of managing transport routes, such as saving, updating, or deleting route records in the database.
    /// </summary>
    public class RouteService : IRouteService
    {
        private readonly SqlHelper _db;
        public RouteService(SqlHelper db) => _db = db;

        /// <summary>
        /// Retrieves a complete list of all transport routes for the current school and session from the database.
        /// </summary>
        public List<RouteViewModel> GetAllRoutes(int companyId, int sessionId)
        {
            var list = new List<RouteViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_Mst_Routes_GetAll", p).Rows)
                    list.Add(MapRoute(row));
            }
            catch (Exception) { }
            return list;
        }

        /// <summary>
        /// Looks up the details of a specific transport route using its unique ID.
        /// </summary>
        public RouteViewModel? GetRouteByID(int id)
        {
            var p = new[] { new SqlParameter("@RouteID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_Routes_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapRoute(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates a transport route record in the database.
        /// </summary>
        public (bool Success, string Message) UpsertRoute(RouteUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@RouteID", req.RouteID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@RouteName", req.RouteName),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_Routes_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a transport route's record from the database.
        /// </summary>
        public (bool Success, string Message) DeleteRoute(int id, int userId)
        {
            try
            {
                var p = new[] { 
                    new SqlParameter("@RouteID", id), 
                    new SqlParameter("@UserID", userId) 
                };
                var dt = _db.ExecuteQuery("sp_Mst_Routes_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Updates whether a transport route is currently active or inactive.
        /// </summary>
        public (bool Success, string Message) ToggleRouteStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@RouteID", id),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_Routes_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// A helper tool that converts raw database data about a route into a format the application can easily use.
        /// </summary>
        private static RouteViewModel MapRoute(DataRow r) => new()
        {
            RouteID = Convert.ToInt32(r["RouteID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            RouteName = r["RouteName"].ToString()!,
            IsActive = Convert.ToBoolean(r["IsActive"]),
            IsDelete = Convert.ToBoolean(r["IsDelete"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"]),
            CreatedBy = Convert.ToInt32(r["CreatedBy"]),
            ModifiedOn = r["ModifiedOn"] == DBNull.Value ? null : Convert.ToDateTime(r["ModifiedOn"]),
            ModifiedBy = r["ModifiedBy"] == DBNull.Value ? null : Convert.ToInt32(r["ModifiedBy"])
        };
    }
}
