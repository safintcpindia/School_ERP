using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class VehicleAssignService : IVehicleAssignService
    {
        private readonly SqlHelper _db;
        public VehicleAssignService(SqlHelper db) => _db = db;

        public List<VehicleAssignViewModel> GetAllAssignments(int companyId, int sessionId)
        {
            var list = new List<VehicleAssignViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_Mst_VehicleAssign_GetAll", p).Rows)
                    list.Add(MapAssignment(row));
            }
            catch (Exception) { }
            return list;
        }

        public VehicleAssignViewModel? GetAssignmentByID(int id)
        {
            var p = new[] { new SqlParameter("@VehicleAssignID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_VehicleAssign_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapAssignment(dt.Rows[0]);
        }

        public (bool Success, string Message) UpsertAssignments(VehicleAssignUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                int successCount = 0;
                string lastMessage = "No vehicles selected";

                foreach (var vehicleId in req.VehicleIDs)
                {
                    var p = new[] {
                        new SqlParameter("@VehicleAssignID", SqlDbType.Int) { Value = 0 },
                        new SqlParameter("@CompanyID", SqlDbType.Int) { Value = companyId },
                        new SqlParameter("@SessionID", SqlDbType.Int) { Value = sessionId },
                        new SqlParameter("@RouteID", SqlDbType.Int) { Value = req.RouteID },
                        new SqlParameter("@VehicleID", SqlDbType.Int) { Value = vehicleId },
                        new SqlParameter("@IsActive", SqlDbType.Bit) { Value = req.IsActive },
                        new SqlParameter("@UserID", SqlDbType.Int) { Value = userId }
                    };
                    var dt = _db.ExecuteQuery("sp_Mst_VehicleAssign_Upsert", p);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var res = Convert.ToInt32(dt.Rows[0]["Result"]);
                        lastMessage = dt.Rows[0]["Message"].ToString()!;
                        if (res == 1) successCount++;
                    }
                }

                if (successCount > 0)
                    return (true, $"{successCount} vehicle(s) assigned successfully");
                
                return (false, lastMessage);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteAssignment(int id, int userId)
        {
            try
            {
                var p = new[] { 
                    new SqlParameter("@VehicleAssignID", id), 
                    new SqlParameter("@UserID", userId) 
                };
                var dt = _db.ExecuteQuery("sp_Mst_VehicleAssign_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) ToggleAssignmentStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@VehicleAssignID", id),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_VehicleAssign_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static VehicleAssignViewModel MapAssignment(DataRow r) => new()
        {
            VehicleAssignID = Convert.ToInt32(r["VehicleAssignID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            RouteID = Convert.ToInt32(r["RouteID"]),
            VehicleID = Convert.ToInt32(r["VehicleID"]),
            RouteName = r.Table.Columns.Contains("RouteName") ? r["RouteName"].ToString()! : "",
            VehicleNumber = r.Table.Columns.Contains("VehicleNumber") ? r["VehicleNumber"].ToString()! : "",
            VehicleModel = r.Table.Columns.Contains("VehicleModel") ? r["VehicleModel"].ToString() : null,
            VehicleDriverName = r.Table.Columns.Contains("VehicleDriverName") ? r["VehicleDriverName"].ToString() : null,
            IsActive = Convert.ToBoolean(r["IsActive"]),
            IsDelete = Convert.ToBoolean(r["IsDelete"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"])
        };
    }
}
