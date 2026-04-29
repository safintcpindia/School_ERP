using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly SqlHelper _db;
        public VehicleService(SqlHelper db) => _db = db;

        public List<VehicleViewModel> GetAllVehicles(int companyId, int sessionId)
        {
            var list = new List<VehicleViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_Mst_Vehicles_GetAll", p).Rows)
                    list.Add(MapVehicle(row));
            }
            catch (Exception) { }
            return list;
        }

        public VehicleViewModel? GetVehicleByID(int id)
        {
            var p = new[] { new SqlParameter("@VehicleID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_Vehicles_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapVehicle(dt.Rows[0]);
        }

        public (bool Success, string Message) UpsertVehicle(VehicleUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@VehicleID", req.VehicleID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@VehicleNumber", req.VehicleNumber),
                    new SqlParameter("@VehicleModel", (object?)req.VehicleModel ?? DBNull.Value),
                    new SqlParameter("@VehicleYearMade", (object?)req.VehicleYearMade ?? DBNull.Value),
                    new SqlParameter("@VehicleRegNo", (object?)req.VehicleRegNo ?? DBNull.Value),
                    new SqlParameter("@VehicleChasisNo", (object?)req.VehicleChasisNo ?? DBNull.Value),
                    new SqlParameter("@VehicleMaxCapicity", (object?)req.VehicleMaxCapicity ?? DBNull.Value),
                    new SqlParameter("@VehicleDriverName", (object?)req.VehicleDriverName ?? DBNull.Value),
                    new SqlParameter("@VehicleDriverLicense", (object?)req.VehicleDriverLicense ?? DBNull.Value),
                    new SqlParameter("@VehicleDriverContact", (object?)req.VehicleDriverContact ?? DBNull.Value),
                    new SqlParameter("@VehicleDriverPhotoAttach", (object?)req.VehicleDriverPhotoAttach ?? DBNull.Value),
                    new SqlParameter("@VehicleDriverPhotoName", (object?)req.VehicleDriverPhotoName ?? DBNull.Value),
                    new SqlParameter("@VehicleDriverPhotoType", (object?)req.VehicleDriverPhotoType ?? DBNull.Value),
                    new SqlParameter("@VehicleNote", (object?)req.VehicleNote ?? DBNull.Value),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_Vehicles_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteVehicle(int id, int userId)
        {
            try
            {
                var p = new[] { 
                    new SqlParameter("@VehicleID", id), 
                    new SqlParameter("@UserID", userId) 
                };
                var dt = _db.ExecuteQuery("sp_Mst_Vehicles_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) ToggleVehicleStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@VehicleID", id),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_Vehicles_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static VehicleViewModel MapVehicle(DataRow r) => new()
        {
            VehicleID = Convert.ToInt32(r["VehicleID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            VehicleNumber = r["VehicleNumber"].ToString()!,
            VehicleModel = r["VehicleModel"] == DBNull.Value ? null : r["VehicleModel"].ToString(),
            VehicleYearMade = r["VehicleYearMade"] == DBNull.Value ? null : r["VehicleYearMade"].ToString(),
            VehicleRegNo = r["VehicleRegNo"] == DBNull.Value ? null : r["VehicleRegNo"].ToString(),
            VehicleChasisNo = r["VehicleChasisNo"] == DBNull.Value ? null : r["VehicleChasisNo"].ToString(),
            VehicleMaxCapicity = r["VehicleMaxCapicity"] == DBNull.Value ? null : Convert.ToInt32(r["VehicleMaxCapicity"]),
            VehicleDriverName = r["VehicleDriverName"] == DBNull.Value ? null : r["VehicleDriverName"].ToString(),
            VehicleDriverLicense = r["VehicleDriverLicense"] == DBNull.Value ? null : r["VehicleDriverLicense"].ToString(),
            VehicleDriverContact = r["VehicleDriverContact"] == DBNull.Value ? null : r["VehicleDriverContact"].ToString(),
            VehicleDriverPhotoAttach = r["VehicleDriverPhotoAttach"] == DBNull.Value ? null : (byte[])r["VehicleDriverPhotoAttach"],
            VehicleDriverPhotoName = r["VehicleDriverPhotoName"] == DBNull.Value ? null : r["VehicleDriverPhotoName"].ToString(),
            VehicleDriverPhotoType = r["VehicleDriverPhotoType"] == DBNull.Value ? null : r["VehicleDriverPhotoType"].ToString(),
            VehicleNote = r["VehicleNote"] == DBNull.Value ? null : r["VehicleNote"].ToString(),
            IsActive = Convert.ToBoolean(r["IsActive"]),
            IsDelete = Convert.ToBoolean(r["IsDelete"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"]),
            CreatedBy = Convert.ToInt32(r["CreatedBy"]),
            ModifiedOn = r["ModifiedOn"] == DBNull.Value ? null : Convert.ToDateTime(r["ModifiedOn"]),
            ModifiedBy = r["ModifiedBy"] == DBNull.Value ? null : Convert.ToInt32(r["ModifiedBy"])
        };
    }
}
