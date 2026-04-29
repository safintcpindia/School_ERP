using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class PickupPointService : IPickupPointService
    {
        private readonly SqlHelper _db;
        public PickupPointService(SqlHelper db) => _db = db;

        public List<PickupPointViewModel> GetAllPickupPoints(int companyId, int sessionId)
        {
            var list = new List<PickupPointViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_Mst_PickupPoint_GetAll", p).Rows)
                    list.Add(MapPickupPoint(row));
            }
            catch (Exception) { }
            return list;
        }

        public PickupPointViewModel? GetPickupPointByID(int id)
        {
            var p = new[] { new SqlParameter("@PickupPointID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_PickupPoint_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapPickupPoint(dt.Rows[0]);
        }

        public (bool Success, string Message) UpsertPickupPoint(PickupPointUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@PickupPointID", req.PickupPointID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@PickupPointName", req.PickupPointName),
                    new SqlParameter("@PickupPointLatitude", (object?)req.PickupPointLatitude ?? DBNull.Value),
                    new SqlParameter("@PickupPointLongitude", (object?)req.PickupPointLongitude ?? DBNull.Value),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_PickupPoint_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeletePickupPoint(int id, int userId)
        {
            try
            {
                var p = new[] { 
                    new SqlParameter("@PickupPointID", id), 
                    new SqlParameter("@UserID", userId) 
                };
                var dt = _db.ExecuteQuery("sp_Mst_PickupPoint_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) TogglePickupPointStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@PickupPointID", id),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_PickupPoint_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static PickupPointViewModel MapPickupPoint(DataRow r) => new()
        {
            PickupPointID = Convert.ToInt32(r["PickupPointID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            PickupPointName = r["PickupPointName"].ToString()!,
            PickupPointLatitude = r["PickupPointLatitude"] == DBNull.Value ? null : Convert.ToDecimal(r["PickupPointLatitude"]),
            PickupPointLongitude = r["PickupPointLongitude"] == DBNull.Value ? null : Convert.ToDecimal(r["PickupPointLongitude"]),
            IsActive = Convert.ToBoolean(r["IsActive"]),
            IsDelete = Convert.ToBoolean(r["IsDelete"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"]),
            CreatedBy = Convert.ToInt32(r["CreatedBy"]),
            ModifiedOn = r["ModifiedOn"] == DBNull.Value ? null : Convert.ToDateTime(r["ModifiedOn"]),
            ModifiedBy = r["ModifiedBy"] == DBNull.Value ? null : Convert.ToInt32(r["ModifiedBy"])
        };
    }
}
