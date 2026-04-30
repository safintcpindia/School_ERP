using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This service handles the actual work of managing hostel facilities, such as saving, updating, or deleting hostel buildings and room records in the database.
    /// </summary>
    public class HostelService : IHostelService
    {
        private readonly SqlHelper _db;
        public HostelService(SqlHelper db) => _db = db;

        // ─── ROOM TYPE ──────────────────────────────────────────
        /// <summary>
        /// Retrieves a complete list of all room categories for the current school and session from the database.
        /// </summary>
        public List<RoomTypeViewModel> GetAllRoomTypes(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<RoomTypeViewModel>();
            
            // Step 1: Prepare the specific details (school, session, deleted status) we want to look for.
            var p = new[] {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            
            // Step 2: Ask the database for all matching room types using the 'GetAll' recipe.
            // Step 3: For each record found, convert it into a format the application understands and add it to our list.
            foreach (DataRow row in _db.ExecuteQuery("sp_Mst_RoomType_GetAll", p).Rows)
                list.Add(MapRoomType(row));
                
            return list;
        }

        /// <summary>
        /// Looks up the details of a specific room type using its unique ID.
        /// </summary>
        public RoomTypeViewModel? GetRoomTypeByID(int id)
        {
            var p = new[] { new SqlParameter("@RoomTypeID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_RoomType_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapRoomType(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates a room type record in the database.
        /// </summary>
        public (bool Success, string Message) UpsertRoomType(RoomTypeUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                // Step 1: Bundle all the new information about the room type (Title, Description, etc.).
                var p = new[] {
                    new SqlParameter("@RoomTypeID", req.RoomTypeID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@RoomTypeTitle", req.RoomTypeTitle),
                    new SqlParameter("@RoomTypeDescription", (object?)req.RoomTypeDescription ?? DBNull.Value),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                
                // Step 2: Send this bundle to the database to either create a new one or update the existing one.
                var dt = _db.ExecuteQuery("sp_Mst_RoomType_Upsert", p);
                
                // Step 3: Return whether it worked and what the database said (e.g., 'Saved successfully').
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) 
            { 
                // If there's a technical error, report it back.
                return (false, ex.Message); 
            }
        }

        /// <summary>
        /// Deletes a room type's record from the database.
        /// </summary>
        public (bool Success, string Message) DeleteRoomType(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@RoomTypeID", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_Mst_RoomType_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Updates whether a room type is currently active or inactive.
        /// </summary>
        public (bool Success, string Message) ToggleRoomTypeStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@RoomTypeID", id),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_RoomType_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static RoomTypeViewModel MapRoomType(DataRow r) => new()
        {
            RoomTypeID = Convert.ToInt32(r["RoomTypeID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            RoomTypeTitle = r["RoomTypeTitle"].ToString()!,
            RoomTypeDescription = r["RoomTypeDescription"] == DBNull.Value ? null : r["RoomTypeDescription"].ToString(),
            IsActive = Convert.ToBoolean(r["IsActive"]),
            IsDelete = Convert.ToBoolean(r["IsDelete"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"]),
            CreatedBy = Convert.ToInt32(r["CreatedBy"]),
            ModifiedOn = r["ModifiedOn"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["ModifiedOn"]),
            ModifiedBy = r["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["ModifiedBy"])
        };

        // ─── HOSTEL ─────────────────────────────────────────────
        /// <summary>
        /// Retrieves a complete list of all hostel buildings for the current school and session from the database.
        /// </summary>
        public List<HostelViewModel> GetAllHostels(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<HostelViewModel>();
            var p = new[] {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            foreach (DataRow row in _db.ExecuteQuery("sp_Mst_Hostel_GetAll", p).Rows)
                list.Add(MapHostel(row));
            return list;
        }

        /// <summary>
        /// Looks up the details of a specific hostel building using its unique ID.
        /// </summary>
        public HostelViewModel? GetHostelByID(int id)
        {
            var p = new[] { new SqlParameter("@HostelID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_Hostel_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapHostel(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates a hostel building record in the database.
        /// </summary>
        public (bool Success, string Message) UpsertHostel(HostelUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                // Step 1: Prepare all the details about the hostel building.
                var p = new[] {
                    new SqlParameter("@HostelID", req.HostelID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@HostelName", req.HostelName),
                    new SqlParameter("@RoomTypeID", req.RoomTypeID),
                    new SqlParameter("@HostelAddress", (object?)req.HostelAddress ?? DBNull.Value),
                    new SqlParameter("@HostelIntake", req.HostelIntake),
                    new SqlParameter("@HostelDescription", (object?)req.HostelDescription ?? DBNull.Value),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                
                // Step 2: Send the hostel building details to the database to save or update.
                var dt = _db.ExecuteQuery("sp_Mst_Hostel_Upsert", p);
                
                // Step 3: Tell the user if the building record was saved successfully.
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a hostel building's record from the database.
        /// </summary>
        public (bool Success, string Message) DeleteHostel(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@HostelID", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_Mst_Hostel_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Updates whether a hostel building is currently active or inactive.
        /// </summary>
        public (bool Success, string Message) ToggleHostelStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@HostelID", id),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_Hostel_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static HostelViewModel MapHostel(DataRow r) => new()
        {
            HostelID = Convert.ToInt32(r["HostelID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            HostelName = r["HostelName"].ToString()!,
            RoomTypeID = Convert.ToInt32(r["RoomTypeID"]),
            RoomTypeTitle = r.Table.Columns.Contains("RoomTypeTitle") ? r["RoomTypeTitle"].ToString() : null,
            HostelAddress = r["HostelAddress"] == DBNull.Value ? null : r["HostelAddress"].ToString(),
            HostelIntake = Convert.ToInt32(r["HostelIntake"]),
            HostelDescription = r["HostelDescription"] == DBNull.Value ? null : r["HostelDescription"].ToString(),
            IsActive = Convert.ToBoolean(r["IsActive"]),
            IsDelete = Convert.ToBoolean(r["IsDelete"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"]),
            CreatedBy = Convert.ToInt32(r["CreatedBy"]),
            ModifiedOn = r["ModifiedOn"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["ModifiedOn"]),
            ModifiedBy = r["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["ModifiedBy"])
        };

        // ─── HOSTEL ROOM ────────────────────────────────────────
        /// <summary>
        /// Retrieves a complete list of all individual hostel rooms for the current school and session from the database.
        /// </summary>
        public List<HostelRoomViewModel> GetAllHostelRooms(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<HostelRoomViewModel>();
            var p = new[] {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            foreach (DataRow row in _db.ExecuteQuery("sp_Mst_HostelRoom_GetAll", p).Rows)
                list.Add(MapHostelRoom(row));
            return list;
        }

        /// <summary>
        /// Looks up the details of a specific hostel room using its unique ID.
        /// </summary>
        public HostelRoomViewModel? GetHostelRoomByID(int id)
        {
            var p = new[] { new SqlParameter("@RoomId", id) };
            var dt = _db.ExecuteQuery("sp_Mst_HostelRoom_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapHostelRoom(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates a hostel room record in the database, including bed count and cost.
        /// </summary>
        public (bool Success, string Message) UpsertHostelRoom(HostelRoomUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                // Step 1: Pack the room details like bed count and cost per bed.
                var p = new[] {
                    new SqlParameter("@RoomId", req.RoomId),
                    new SqlParameter("@HostelID", req.HostelID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@RoomTypeID", req.RoomTypeID),
                    new SqlParameter("@RoomTitle", req.RoomTitle),
                    new SqlParameter("@NoOfBed", req.NoOfBed),
                    new SqlParameter("@CostPerBed", req.CostPerBed),
                    new SqlParameter("@RoomDescription", (object?)req.RoomDescription ?? DBNull.Value),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                
                // Step 2: Update the specific room information in the database.
                var dt = _db.ExecuteQuery("sp_Mst_HostelRoom_Upsert", p);
                
                // Step 3: Inform the user if the room update was successful.
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a hostel room's record from the database.
        /// </summary>
        public (bool Success, string Message) DeleteHostelRoom(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@RoomId", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_Mst_HostelRoom_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Updates whether a hostel room is currently active or inactive.
        /// </summary>
        public (bool Success, string Message) ToggleHostelRoomStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@RoomId", id),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_HostelRoom_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static HostelRoomViewModel MapHostelRoom(DataRow r) => new()
        {
            RoomId = Convert.ToInt32(r["RoomId"]),
            HostelID = Convert.ToInt32(r["HostelID"]),
            HostelName = r.Table.Columns.Contains("HostelName") ? r["HostelName"].ToString() : null,
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            RoomTypeID = Convert.ToInt32(r["RoomTypeID"]),
            RoomTypeTitle = r.Table.Columns.Contains("RoomTypeTitle") ? r["RoomTypeTitle"].ToString() : null,
            RoomTitle = r["RoomTitle"].ToString()!,
            NoOfBed = Convert.ToInt32(r["NoOfBed"]),
            CostPerBed = Convert.ToDecimal(r["CostPerBed"]),
            RoomDescription = r["RoomDescription"] == DBNull.Value ? null : r["RoomDescription"].ToString(),
            IsActive = Convert.ToBoolean(r["IsActive"]),
            IsDelete = Convert.ToBoolean(r["IsDelete"]),
            CreatedBy = Convert.ToInt32(r["CreatedBy"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"]),
            ModifiedBy = r["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["ModifiedBy"]),
            ModifiedOn = r["ModifiedOn"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["ModifiedOn"])
        };
    }
}
