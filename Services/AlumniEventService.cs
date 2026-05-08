using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class AlumniEventService : IAlumniEventService
    {
        private readonly SqlHelper _sqlHelper;

        public AlumniEventService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<AlumniEventViewModel> GetEvents(string? searchText, int companyId)
        {
            var list = new List<AlumniEventViewModel>();
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Action", "LIST"),
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SearchText", (object?)searchText ?? DBNull.Value)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_AlumniEvents_CRUD", parameters.ToArray());
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AlumniEventViewModel
                {
                    EventID = Convert.ToInt32(row["EventID"]),
                    EventTitle = row["EventTitle"].ToString(),
                    EventDescription = row["EventDescription"].ToString(),
                    FromDate = Convert.ToDateTime(row["FromDate"]),
                    ToDate = Convert.ToDateTime(row["ToDate"]),
                    Location = row["Location"].ToString(),
                    EventPhotoName = row["EventPhotoName"].ToString(),
                    EventPhotoType = row["EventPhotoType"].ToString(),
                    HasPhoto = Convert.ToInt32(row["HasPhoto"]),
                    EventFor = Convert.ToInt32(row["EventFor"]),
                    SessionID = row["SessionID"] != DBNull.Value ? Convert.ToInt32(row["SessionID"]) : null,
                    ClassID = row["ClassID"] != DBNull.Value ? Convert.ToInt32(row["ClassID"]) : null,
                    SectionIDs = row["SectionIDs"].ToString(),
                    SessionName = row["SessionTitle"].ToString(),
                    ClassName = row["ClassName"].ToString(),
                    SectionNames = row["SectionNames"].ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedOn"])
                });
            }
            return list;
        }

        public (bool Success, string Message) UpsertEvent(AlumniEventUpsertRequest req, int companyId, int userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Action", "UPSERT"),
                new SqlParameter("@EventID", req.EventID),
                new SqlParameter("@EventTitle", req.EventTitle),
                new SqlParameter("@EventDescription", (object?)req.EventDescription ?? DBNull.Value),
                new SqlParameter("@FromDate", req.FromDate),
                new SqlParameter("@ToDate", req.ToDate),
                new SqlParameter("@Location", (object?)req.Location ?? DBNull.Value),
                new SqlParameter("@EventPhoto", (object?)req.EventPhoto ?? DBNull.Value) { SqlDbType = SqlDbType.VarBinary },
                new SqlParameter("@EventPhotoType", (object?)req.EventPhotoType ?? DBNull.Value),
                new SqlParameter("@EventPhotoName", (object?)req.EventPhotoName ?? DBNull.Value),
                new SqlParameter("@EventFor", req.EventFor),
                new SqlParameter("@SessionID", (object?)req.SessionID ?? DBNull.Value),
                new SqlParameter("@ClassID", (object?)req.ClassID ?? DBNull.Value),
                new SqlParameter("@SectionIDs", (object?)req.SectionIDs ?? DBNull.Value),
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@UserID", userId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_AlumniEvents_CRUD", parameters.ToArray());
            if (dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString());
            }
            return (false, "Operation failed");
        }

        public (bool Success, string Message) DeleteEvent(int eventId, int companyId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Action", "DELETE"),
                new SqlParameter("@EventID", eventId),
                new SqlParameter("@CompanyID", companyId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_AlumniEvents_CRUD", parameters.ToArray());
            if (dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString());
            }
            return (false, "Deletion failed");
        }

        public (bool Success, string Message) ToggleEventStatus(int eventId, bool isActive, int companyId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Action", "TOGGLE_STATUS"),
                new SqlParameter("@EventID", eventId),
                new SqlParameter("@IsActive", isActive),
                new SqlParameter("@CompanyID", companyId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_AlumniEvents_CRUD", parameters.ToArray());
            if (dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString());
            }
            return (false, "Status update failed");
        }

        public (byte[]? Bytes, string? FileName, string? ContentType) GetEventPhoto(int eventId, int companyId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Action", "GET_BY_ID"),
                new SqlParameter("@EventID", eventId),
                new SqlParameter("@CompanyID", companyId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_AlumniEvents_CRUD", parameters.ToArray());
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                if (row["EventPhoto"] != DBNull.Value)
                {
                    return ((byte[])row["EventPhoto"], row["EventPhotoName"].ToString(), row["EventPhotoType"].ToString());
                }
            }
            return (null, null, null);
        }
    }
}
