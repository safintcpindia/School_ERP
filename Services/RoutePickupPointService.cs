using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This service handles the actual work of linking transport routes to specific pickup points and managing trip details like distance and fees.
    /// </summary>
    public class RoutePickupPointService : IRoutePickupPointService
    {
        private readonly SqlHelper _db;
        public RoutePickupPointService(SqlHelper db) => _db = db;

        /// <summary>
        /// Retrieves a complete list of all route-to-pickup point links for the current school and session from the database.
        /// </summary>
        public List<RoutePickupPointViewModel> GetAllRoutePickupPoints(int companyId, int sessionId)
        {
            var list = new List<RoutePickupPointViewModel>();
            try
            {
                // Step 1: Pack the search criteria (School and Session).
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId)
                };
                
                // Step 2: Ask the database for all matching route-to-pickup point links.
                var dt = _db.ExecuteQuery("sp_Mst_RoutePickupPoints_GetAll", p);
                if (dt != null)
                {
                    // Step 3: For each link found, convert it into a format the application can use.
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(MapRoutePickupPoint(row));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
            return list;
        }

        public RoutePickupPointViewModel? GetRoutePickupPointByID(int id)
        {
            var p = new[] { new SqlParameter("@RoutePickupPointID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_RoutePickupPoints_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapRoutePickupPoint(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates a route-to-pickup point link in the database, including trip details like distance and monthly fees.
        /// </summary>
        public (bool Success, string Message) UpsertRoutePickupPoint(RoutePickupPointUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                // Step 1: Bundle all the details for the link (Route, Pickup Point, Distance, Time, and Fees).
                var p = new[] {
                    new SqlParameter("@RoutePickupPointID", SqlDbType.Int) { Value = req.RoutePickupPointID },
                    new SqlParameter("@CompanyID", SqlDbType.Int) { Value = companyId },
                    new SqlParameter("@SessionID", SqlDbType.Int) { Value = sessionId },
                    new SqlParameter("@RouteID", SqlDbType.Int) { Value = req.RouteID },
                    new SqlParameter("@PickupPointID", SqlDbType.Int) { Value = req.PickupPointID },
                    new SqlParameter("@Distance", SqlDbType.Decimal) { Value = (object?)req.Distance ?? DBNull.Value },
                    new SqlParameter("@PickupTime", SqlDbType.NVarChar) { Value = (object?)req.PickupTime ?? DBNull.Value },
                    new SqlParameter("@MonthlyFees", SqlDbType.Decimal) { Value = (object?)req.MonthlyFees ?? DBNull.Value },
                    new SqlParameter("@IsActive", SqlDbType.Bit) { Value = req.IsActive },
                    new SqlParameter("@UserID", SqlDbType.Int) { Value = userId }
                };
                
                // Step 2: Send this bundle to the database to save or update the link record.
                var dt = _db.ExecuteQuery("sp_Mst_RoutePickupPoints_Upsert", p);
                
                // Step 3: Inform the user if the record was saved successfully.
                if (dt != null && dt.Rows.Count > 0)
                {
                    return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
                }
                return (false, "No response from database");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteRoutePickupPoint(int id, int userId)
        {
            try
            {
                var p = new[] { 
                    new SqlParameter("@RoutePickupPointID", SqlDbType.Int) { Value = id }, 
                    new SqlParameter("@UserID", SqlDbType.Int) { Value = userId } 
                };
                var dt = _db.ExecuteQuery("sp_Mst_RoutePickupPoints_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) ToggleRoutePickupPointStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@RoutePickupPointID", SqlDbType.Int) { Value = id },
                    new SqlParameter("@IsActive", SqlDbType.Bit) { Value = isActive },
                    new SqlParameter("@UserID", SqlDbType.Int) { Value = userId }
                };
                var dt = _db.ExecuteQuery("sp_Mst_RoutePickupPoints_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static RoutePickupPointViewModel MapRoutePickupPoint(DataRow r) => new()
        {
            RoutePickupPointID = r.Table.Columns.Contains("RoutePickupPointID") ? Convert.ToInt32(r["RoutePickupPointID"]) : 0,
            CompanyID = r.Table.Columns.Contains("CompanyID") ? Convert.ToInt32(r["CompanyID"]) : 0,
            SessionID = r.Table.Columns.Contains("SessionID") ? Convert.ToInt32(r["SessionID"]) : 0,
            RouteID = r.Table.Columns.Contains("RouteID") ? Convert.ToInt32(r["RouteID"]) : 0,
            PickupPointID = r.Table.Columns.Contains("PickupPointID") ? Convert.ToInt32(r["PickupPointID"]) : 0,
            RouteName = r.Table.Columns.Contains("RouteName") ? r["RouteName"].ToString()! : "Unknown",
            PickupPointName = r.Table.Columns.Contains("PickupPointName") ? r["PickupPointName"].ToString()! : "Unknown",
            Distance = r.Table.Columns.Contains("Distance") && r["Distance"] != DBNull.Value ? Convert.ToDecimal(r["Distance"]) : null,
            PickupTime = r.Table.Columns.Contains("PickupTime") && r["PickupTime"] != DBNull.Value ? r["PickupTime"].ToString() : null,
            MonthlyFees = r.Table.Columns.Contains("MonthlyFees") && r["MonthlyFees"] != DBNull.Value ? Convert.ToDecimal(r["MonthlyFees"]) : null,
            IsActive = r.Table.Columns.Contains("IsActive") && (r["IsActive"] != DBNull.Value && Convert.ToBoolean(r["IsActive"])),
            IsDelete = r.Table.Columns.Contains("IsDelete") && (r["IsDelete"] != DBNull.Value && Convert.ToBoolean(r["IsDelete"])),
            CreatedOn = r.Table.Columns.Contains("CreatedOn") && r["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(r["CreatedOn"]) : DateTime.MinValue
        };
    }
}
