using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This service handles the actual work of managing school classes, such as saving, updating, or deleting class records in the database.
    /// </summary>
    public class ClassService : IClassService
    {
        private readonly SqlHelper _sqlHelper;

        public ClassService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        /// <summary>
        /// Retrieves a complete list of all classes for the current school and session from the database.
        /// </summary>
        public List<MstClassViewModel> GetAllClasses(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<MstClassViewModel>();
            var parameters = new[] 
            { 
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted) 
            };
            var dt = _sqlHelper.ExecuteQuery("sp_Class_GetAll", parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        /// <summary>
        /// Looks up the details of a specific class using its unique ID.
        /// </summary>
        public MstClassViewModel? GetClassByID(int classId)
        {
            var parameters = new[] { new SqlParameter("@ClassID", classId) };
            var dt = _sqlHelper.ExecuteQuery("sp_Class_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        /// <summary>
        /// Saves or updates a class record in the database, including assigning sections to that class.
        /// </summary>
        public (bool success, string message) UpsertClass(MstClassUpsertRequest request, int companyId, int sessionId, int userId)
        {
            try
            {
                string sectionIdsStr = request.SectionIds != null ? string.Join(",", request.SectionIds) : "";
                var parameters = new[]
                {
                    new SqlParameter("@ClassID", request.ClassID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@ClassName", request.ClassName),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@SectionIds", sectionIdsStr)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Class_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a class's record from the database.
        /// </summary>
        public (bool success, string message) DeleteClass(int classId, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@ClassID", classId),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Class_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Updates whether a class is currently active or inactive.
        /// </summary>
        public (bool success, string message) ToggleClassStatus(int classId, bool isActive, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@ClassID", classId),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Class_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// A helper tool that converts raw database data about a class into a format the application can easily use.
        /// </summary>
        private MstClassViewModel MapRowToViewModel(DataRow row)
        {
            var model = new MstClassViewModel
            {
                ClassID = Convert.ToInt32(row["ClassID"]),
                CompanyID = Convert.ToInt32(row["CompanyID"]),
                SessionID = Convert.ToInt32(row["SessionID"]),
                ClassName = row["ClassName"].ToString() ?? "",
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                ModifiedBy = row["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(row["ModifiedBy"]) : null,
                ModifiedOn = row["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedOn"]) : null
            };

            if (row.Table.Columns.Contains("SectionNames"))
            {
                model.SectionNames = row["SectionNames"].ToString() ?? "";
            }

            if (row.Table.Columns.Contains("SectionIds"))
            {
                string ids = row["SectionIds"].ToString() ?? "";
                if (!string.IsNullOrEmpty(ids))
                {
                    model.SectionIds = ids.Split(',').Select(int.Parse).ToList();
                }
            }

            return model;
        }
    }
}
