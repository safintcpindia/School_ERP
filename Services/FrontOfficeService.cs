using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class FrontOfficeService : IFrontOfficeService
    {
        private readonly SqlHelper _db;
        public FrontOfficeService(SqlHelper db) => _db = db;

        // ─── HELPERS ────────────────────────────────────────────
        private static T? SafeGet<T>(DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col) || row[col] == DBNull.Value) return default;
            return (T)Convert.ChangeType(row[col], typeof(T));
        }

        // ─── PURPOSE ────────────────────────────────────────────
        public List<MstFOPurposeViewModel> GetAllPurposes(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<MstFOPurposeViewModel>();
            var p = new[] {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            foreach (DataRow row in _db.ExecuteQuery("sp_FO_Purpose_GetAll", p).Rows)
                list.Add(MapPurpose(row));
            return list;
        }

        public MstFOPurposeViewModel? GetPurposeByID(int id)
        {
            var p = new[] { new SqlParameter("@PurposeID", id) };
            var dt = _db.ExecuteQuery("sp_FO_Purpose_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapPurpose(dt.Rows[0]);
        }

        public (bool, string) UpsertPurpose(MstFOPurposeUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@PurposeID", req.PurposeID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@Name", req.Name),
                    new SqlParameter("@Description", (object?)req.Description ?? DBNull.Value),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _db.ExecuteQuery("sp_FO_Purpose_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) DeletePurpose(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@PurposeID", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_FO_Purpose_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) TogglePurposeStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@PurposeID", id), new SqlParameter("@IsActive", isActive), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_FO_Purpose_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static MstFOPurposeViewModel MapPurpose(DataRow r) => new()
        {
            PurposeID = Convert.ToInt32(r["PurposeID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            Name = r["Name"].ToString()!,
            Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
            IsActive = Convert.ToBoolean(r["IsActive"]),
            CreatedOn = r.Table.Columns.Contains("CreatedOn") && r["CreatedOn"] != DBNull.Value
                        ? Convert.ToDateTime(r["CreatedOn"]) : DateTime.MinValue
        };

        // ─── COMPLAINT TYPE ─────────────────────────────────────
        public List<MstFOComplaintTypeViewModel> GetAllComplaintTypes(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<MstFOComplaintTypeViewModel>();
            var p = new[] {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            foreach (DataRow row in _db.ExecuteQuery("sp_FO_ComplaintType_GetAll", p).Rows)
                list.Add(MapComplaintType(row));
            return list;
        }

        public MstFOComplaintTypeViewModel? GetComplaintTypeByID(int id)
        {
            var p = new[] { new SqlParameter("@ComplaintTypeID", id) };
            var dt = _db.ExecuteQuery("sp_FO_ComplaintType_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapComplaintType(dt.Rows[0]);
        }

        public (bool, string) UpsertComplaintType(MstFOComplaintTypeUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@ComplaintTypeID", req.ComplaintTypeID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@Name", req.Name),
                    new SqlParameter("@Description", (object?)req.Description ?? DBNull.Value),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _db.ExecuteQuery("sp_FO_ComplaintType_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) DeleteComplaintType(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@ComplaintTypeID", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_FO_ComplaintType_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) ToggleComplaintTypeStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@ComplaintTypeID", id), new SqlParameter("@IsActive", isActive), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_FO_ComplaintType_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static MstFOComplaintTypeViewModel MapComplaintType(DataRow r) => new()
        {
            ComplaintTypeID = Convert.ToInt32(r["ComplaintTypeID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            Name = r["Name"].ToString()!,
            Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
            IsActive = Convert.ToBoolean(r["IsActive"]),
            CreatedOn = r.Table.Columns.Contains("CreatedOn") && r["CreatedOn"] != DBNull.Value
                        ? Convert.ToDateTime(r["CreatedOn"]) : DateTime.MinValue
        };

        // ─── SOURCE ─────────────────────────────────────────────
        public List<MstFOSourceViewModel> GetAllSources(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<MstFOSourceViewModel>();
            var p = new[] {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            foreach (DataRow row in _db.ExecuteQuery("sp_FO_Source_GetAll", p).Rows)
                list.Add(MapSource(row));
            return list;
        }

        public MstFOSourceViewModel? GetSourceByID(int id)
        {
            var p = new[] { new SqlParameter("@SourceID", id) };
            var dt = _db.ExecuteQuery("sp_FO_Source_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapSource(dt.Rows[0]);
        }

        public (bool, string) UpsertSource(MstFOSourceUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@SourceID", req.SourceID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@Name", req.Name),
                    new SqlParameter("@Description", (object?)req.Description ?? DBNull.Value),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _db.ExecuteQuery("sp_FO_Source_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) DeleteSource(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@SourceID", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_FO_Source_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) ToggleSourceStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@SourceID", id), new SqlParameter("@IsActive", isActive), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_FO_Source_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static MstFOSourceViewModel MapSource(DataRow r) => new()
        {
            SourceID = Convert.ToInt32(r["SourceID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            Name = r["Name"].ToString()!,
            Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
            IsActive = Convert.ToBoolean(r["IsActive"]),
            CreatedOn = r.Table.Columns.Contains("CreatedOn") && r["CreatedOn"] != DBNull.Value
                        ? Convert.ToDateTime(r["CreatedOn"]) : DateTime.MinValue
        };

        // ─── REFERENCE ──────────────────────────────────────────
        public List<MstFOReferenceViewModel> GetAllReferences(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<MstFOReferenceViewModel>();
            var p = new[] {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            foreach (DataRow row in _db.ExecuteQuery("sp_FO_Reference_GetAll", p).Rows)
                list.Add(MapReference(row));
            return list;
        }

        public MstFOReferenceViewModel? GetReferenceByID(int id)
        {
            var p = new[] { new SqlParameter("@ReferenceID", id) };
            var dt = _db.ExecuteQuery("sp_FO_Reference_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapReference(dt.Rows[0]);
        }

        public (bool, string) UpsertReference(MstFOReferenceUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@ReferenceID", req.ReferenceID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@Name", req.Name),
                    new SqlParameter("@Description", (object?)req.Description ?? DBNull.Value),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _db.ExecuteQuery("sp_FO_Reference_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) DeleteReference(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@ReferenceID", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_FO_Reference_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) ToggleReferenceStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@ReferenceID", id), new SqlParameter("@IsActive", isActive), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_FO_Reference_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static MstFOReferenceViewModel MapReference(DataRow r) => new()
        {
            ReferenceID = Convert.ToInt32(r["ReferenceID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            Name = r["Name"].ToString()!,
            Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
            IsActive = Convert.ToBoolean(r["IsActive"]),
            CreatedOn = r.Table.Columns.Contains("CreatedOn") && r["CreatedOn"] != DBNull.Value
                        ? Convert.ToDateTime(r["CreatedOn"]) : DateTime.MinValue
        };

        // ─── COMPLAINT ──────────────────────────────────────────
        public List<FOComplaintViewModel> GetAllComplaints(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<FOComplaintViewModel>();
            var p = new[] {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            foreach (DataRow row in _db.ExecuteQuery("sp_FO_Complaint_GetAll", p).Rows)
                list.Add(MapComplaint(row));
            return list;
        }

        public FOComplaintViewModel? GetComplaintByID(int id)
        {
            var p = new[] { new SqlParameter("@ComplaintID", id) };
            var dt = _db.ExecuteQuery("sp_FO_Complaint_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapComplaint(dt.Rows[0]);
        }

        public (bool, string) UpsertComplaint(FOComplaintUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@ComplaintID",     req.ComplaintID),
                    new SqlParameter("@CompanyID",       companyId),
                    new SqlParameter("@SessionID",       sessionId),
                    new SqlParameter("@ComplaintTypeID", req.ComplaintTypeID),
                    new SqlParameter("@SourceID",        req.SourceID),
                    new SqlParameter("@ComplaintBy",     req.ComplaintBy),
                    new SqlParameter("@Phone",           (object?)req.Phone       ?? DBNull.Value),
                    new SqlParameter("@Email",           (object?)req.Email       ?? DBNull.Value),
                    new SqlParameter("@ComplaintDate",   req.ComplaintDate),
                    new SqlParameter("@Description",     (object?)req.Description ?? DBNull.Value),
                    new SqlParameter("@ActionTaken",     (object?)req.ActionTaken ?? DBNull.Value),
                    new SqlParameter("@AssignedTo",      (object?)req.AssignedTo  ?? DBNull.Value),
                    new SqlParameter("@Note",            (object?)req.Note        ?? DBNull.Value),
                    new SqlParameter("@Attachment",      (object?)req.Attachment  ?? DBNull.Value),
                    new SqlParameter("@IsActive",        req.IsActive),
                    new SqlParameter("@UserId",          userId)
                };
                var dt = _db.ExecuteQuery("sp_FO_Complaint_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) DeleteComplaint(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@ComplaintID", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_FO_Complaint_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) ToggleComplaintStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@ComplaintID", id),
                    new SqlParameter("@IsActive",    isActive),
                    new SqlParameter("@UserId",      userId)
                };
                var dt = _db.ExecuteQuery("sp_FO_Complaint_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static FOComplaintViewModel MapComplaint(DataRow r) => new()
        {
            ComplaintID      = Convert.ToInt32(r["ComplaintID"]),
            CompanyID        = Convert.ToInt32(r["CompanyID"]),
            SessionID        = Convert.ToInt32(r["SessionID"]),
            ComplaintTypeID  = Convert.ToInt32(r["ComplaintTypeID"]),
            SourceID         = Convert.ToInt32(r["SourceID"]),
            ComplaintTypeName = r.Table.Columns.Contains("ComplaintTypeName") && r["ComplaintTypeName"] != DBNull.Value
                                ? r["ComplaintTypeName"].ToString()! : string.Empty,
            SourceName       = r.Table.Columns.Contains("SourceName") && r["SourceName"] != DBNull.Value
                                ? r["SourceName"].ToString()! : string.Empty,
            ComplaintBy      = r["ComplaintBy"].ToString()!,
            Phone            = r["Phone"] == DBNull.Value ? null : r["Phone"].ToString(),
            Email            = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
            ComplaintDate    = Convert.ToDateTime(r["ComplaintDate"]),
            Description      = r["Description"]  == DBNull.Value ? null : r["Description"].ToString(),
            ActionTaken      = r["ActionTaken"]   == DBNull.Value ? null : r["ActionTaken"].ToString(),
            AssignedTo       = r["AssignedTo"]    == DBNull.Value ? null : r["AssignedTo"].ToString(),
            Note             = r["Note"]          == DBNull.Value ? null : r["Note"].ToString(),
            Attachment       = r["Attachment"]    == DBNull.Value ? null : r["Attachment"].ToString(),
            IsActive         = Convert.ToBoolean(r["IsActive"]),
            CreatedOn        = r.Table.Columns.Contains("CreatedOn") && r["CreatedOn"] != DBNull.Value
                               ? Convert.ToDateTime(r["CreatedOn"]) : DateTime.MinValue
        };

        // ─── POSTAL RECEIVE ─────────────────────────────────────
        public List<FOPostalReceiveViewModel> GetAllPostalReceives(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<FOPostalReceiveViewModel>();
            var p = new[] {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            foreach (DataRow row in _db.ExecuteQuery("sp_FO_PostalReceive_GetAll", p).Rows)
                list.Add(MapPostalReceive(row));
            return list;
        }

        public FOPostalReceiveViewModel? GetPostalReceiveByID(int id)
        {
            var p = new[] { new SqlParameter("@PostalReceiveID", id) };
            var dt = _db.ExecuteQuery("sp_FO_PostalReceive_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapPostalReceive(dt.Rows[0]);
        }

        public (bool, string) UpsertPostalReceive(FOPostalReceiveUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@PostalReceiveID", req.PostalReceiveID),
                    new SqlParameter("@CompanyID",       companyId),
                    new SqlParameter("@SessionID",       sessionId),
                    new SqlParameter("@FromTitle",       req.FromTitle),
                    new SqlParameter("@ToTitle",         (object?)req.ToTitle     ?? DBNull.Value),
                    new SqlParameter("@ReferenceNo",     (object?)req.ReferenceNo ?? DBNull.Value),
                    new SqlParameter("@Address",         (object?)req.Address     ?? DBNull.Value),
                    new SqlParameter("@Note",            (object?)req.Note        ?? DBNull.Value),
                    new SqlParameter("@Date",            req.Date),
                    new SqlParameter("@Attachment",      (object?)req.Attachment  ?? DBNull.Value) { SqlDbType = SqlDbType.VarBinary },
                    new SqlParameter("@FileName",        (object?)req.FileName    ?? DBNull.Value),
                    new SqlParameter("@FileType",        (object?)req.FileType    ?? DBNull.Value),
                    new SqlParameter("@IsActive",        req.IsActive),
                    new SqlParameter("@UserId",          userId)
                };
                var dt = _db.ExecuteQuery("sp_FO_PostalReceive_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) DeletePostalReceive(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@PostalReceiveID", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_FO_PostalReceive_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) TogglePostalReceiveStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@PostalReceiveID", id),
                    new SqlParameter("@IsActive",        isActive),
                    new SqlParameter("@UserId",          userId)
                };
                var dt = _db.ExecuteQuery("sp_FO_PostalReceive_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static FOPostalReceiveViewModel MapPostalReceive(DataRow r) => new()
        {
            PostalReceiveID = Convert.ToInt32(r["PostalReceiveID"]),
            CompanyID       = Convert.ToInt32(r["CompanyID"]),
            SessionID       = Convert.ToInt32(r["SessionID"]),
            FromTitle       = r["FromTitle"].ToString()!,
            ToTitle         = r["ToTitle"] == DBNull.Value ? null : r["ToTitle"].ToString(),
            ReferenceNo     = r["ReferenceNo"] == DBNull.Value ? null : r["ReferenceNo"].ToString(),
            Address         = r["Address"] == DBNull.Value ? null : r["Address"].ToString(),
            Note            = r["Note"] == DBNull.Value ? null : r["Note"].ToString(),
            Date            = Convert.ToDateTime(r["Date"]),
            Attachment      = r["Attachment"] == DBNull.Value ? null : (byte[])r["Attachment"],
            FileName        = r["FileName"] == DBNull.Value ? null : r["FileName"].ToString(),
            FileType        = r["FileType"] == DBNull.Value ? null : r["FileType"].ToString(),
            IsActive        = Convert.ToBoolean(r["IsActive"]),
            CreatedOn       = r.Table.Columns.Contains("CreatedOn") && r["CreatedOn"] != DBNull.Value
                               ? Convert.ToDateTime(r["CreatedOn"]) : DateTime.MinValue
        };

        // ─── POSTAL DISPATCH ────────────────────────────────────
        public List<FOPostalDispatchViewModel> GetAllPostalDispatches(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<FOPostalDispatchViewModel>();
            var p = new[] {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            foreach (DataRow row in _db.ExecuteQuery("sp_FO_PostalDispatch_GetAll", p).Rows)
                list.Add(MapPostalDispatch(row));
            return list;
        }

        public FOPostalDispatchViewModel? GetPostalDispatchByID(int id)
        {
            var p = new[] { new SqlParameter("@PostalDispatchID", id) };
            var dt = _db.ExecuteQuery("sp_FO_PostalDispatch_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapPostalDispatch(dt.Rows[0]);
        }

        public (bool, string) UpsertPostalDispatch(FOPostalDispatchUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@PostalDispatchID", req.PostalDispatchID),
                    new SqlParameter("@CompanyID",        companyId),
                    new SqlParameter("@SessionID",        sessionId),
                    new SqlParameter("@ToTitle",          req.ToTitle),
                    new SqlParameter("@FromTitle",        (object?)req.FromTitle   ?? DBNull.Value),
                    new SqlParameter("@ReferenceNo",      (object?)req.ReferenceNo ?? DBNull.Value),
                    new SqlParameter("@Address",          (object?)req.Address     ?? DBNull.Value),
                    new SqlParameter("@Note",             (object?)req.Note        ?? DBNull.Value),
                    new SqlParameter("@Date",             req.Date),
                    new SqlParameter("@Attachment",       (object?)req.Attachment  ?? DBNull.Value) { SqlDbType = SqlDbType.VarBinary },
                    new SqlParameter("@FileName",         (object?)req.FileName    ?? DBNull.Value),
                    new SqlParameter("@FileType",         (object?)req.FileType    ?? DBNull.Value),
                    new SqlParameter("@IsActive",         req.IsActive),
                    new SqlParameter("@UserId",           userId)
                };
                var dt = _db.ExecuteQuery("sp_FO_PostalDispatch_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) DeletePostalDispatch(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@PostalDispatchID", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_FO_PostalDispatch_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) TogglePostalDispatchStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@PostalDispatchID", id),
                    new SqlParameter("@IsActive",         isActive),
                    new SqlParameter("@UserId",           userId)
                };
                var dt = _db.ExecuteQuery("sp_FO_PostalDispatch_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static FOPostalDispatchViewModel MapPostalDispatch(DataRow r) => new()
        {
            PostalDispatchID = Convert.ToInt32(r["PostalDispatchID"]),
            CompanyID        = Convert.ToInt32(r["CompanyID"]),
            SessionID        = Convert.ToInt32(r["SessionID"]),
            ToTitle          = r["ToTitle"].ToString()!,
            FromTitle        = r["FromTitle"]   == DBNull.Value ? null : r["FromTitle"].ToString(),
            ReferenceNo      = r["ReferenceNo"] == DBNull.Value ? null : r["ReferenceNo"].ToString(),
            Address          = r["Address"]     == DBNull.Value ? null : r["Address"].ToString(),
            Note             = r["Note"]        == DBNull.Value ? null : r["Note"].ToString(),
            Date             = Convert.ToDateTime(r["Date"]),
            Attachment       = r["Attachment"]  == DBNull.Value ? null : (byte[])r["Attachment"],
            FileName         = r["FileName"]    == DBNull.Value ? null : r["FileName"].ToString(),
            FileType         = r["FileType"]    == DBNull.Value ? null : r["FileType"].ToString(),
            IsActive         = Convert.ToBoolean(r["IsActive"]),
            CreatedOn        = r.Table.Columns.Contains("CreatedOn") && r["CreatedOn"] != DBNull.Value
                               ? Convert.ToDateTime(r["CreatedOn"]) : DateTime.MinValue
        };

        // ─── PHONE CALL LOG ─────────────────────────────────────
        public List<FOPhoneCallLogViewModel> GetAllPhoneCallLogs(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<FOPhoneCallLogViewModel>();
            var p = new[] {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            foreach (DataRow row in _db.ExecuteQuery("sp_FO_PhoneCallLog_GetAll", p).Rows)
                list.Add(MapPhoneCallLog(row));
            return list;
        }

        public FOPhoneCallLogViewModel? GetPhoneCallLogByID(int id)
        {
            var p = new[] { new SqlParameter("@PhoneCallLogID", id) };
            var dt = _db.ExecuteQuery("sp_FO_PhoneCallLog_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapPhoneCallLog(dt.Rows[0]);
        }

        public (bool, string) UpsertPhoneCallLog(FOPhoneCallLogUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@PhoneCallLogID",   req.PhoneCallLogID),
                    new SqlParameter("@CompanyID",        companyId),
                    new SqlParameter("@SessionID",        sessionId),
                    new SqlParameter("@Name",             req.Name),
                    new SqlParameter("@Phone",            (object?)req.Phone            ?? DBNull.Value),
                    new SqlParameter("@Date",             req.Date),
                    new SqlParameter("@Description",      (object?)req.Description      ?? DBNull.Value),
                    new SqlParameter("@NextFollowUpDate", (object?)req.NextFollowUpDate ?? DBNull.Value),
                    new SqlParameter("@CallDuration",     (object?)req.CallDuration     ?? DBNull.Value),
                    new SqlParameter("@Note",             (object?)req.Note             ?? DBNull.Value),
                    new SqlParameter("@CallType",         (object?)req.CallType         ?? DBNull.Value),
                    new SqlParameter("@IsActive",         req.IsActive),
                    new SqlParameter("@UserId",           userId)
                };
                var dt = _db.ExecuteQuery("sp_FO_PhoneCallLog_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) DeletePhoneCallLog(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@PhoneCallLogID", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_FO_PhoneCallLog_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool, string) TogglePhoneCallLogStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@PhoneCallLogID", id),
                    new SqlParameter("@IsActive",       isActive),
                    new SqlParameter("@UserId",         userId)
                };
                var dt = _db.ExecuteQuery("sp_FO_PhoneCallLog_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static FOPhoneCallLogViewModel MapPhoneCallLog(DataRow r) => new()
        {
            PhoneCallLogID   = Convert.ToInt32(r["PhoneCallLogID"]),
            CompanyID        = Convert.ToInt32(r["CompanyID"]),
            SessionID        = Convert.ToInt32(r["SessionID"]),
            Name             = r["Name"].ToString()!,
            Phone            = r["Phone"]            == DBNull.Value ? null : r["Phone"].ToString(),
            Date             = Convert.ToDateTime(r["Date"]),
            Description      = r["Description"]      == DBNull.Value ? null : r["Description"].ToString(),
            NextFollowUpDate = r["NextFollowUpDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["NextFollowUpDate"]),
            CallDuration     = r["CallDuration"]     == DBNull.Value ? null : r["CallDuration"].ToString(),
            Note             = r["Note"]             == DBNull.Value ? null : r["Note"].ToString(),
            CallType         = r["CallType"]         == DBNull.Value ? null : r["CallType"].ToString(),
            IsActive         = Convert.ToBoolean(r["IsActive"]),
            CreatedOn        = r.Table.Columns.Contains("CreatedOn") && r["CreatedOn"] != DBNull.Value
                               ? Convert.ToDateTime(r["CreatedOn"]) : DateTime.MinValue
        };
    }
}
