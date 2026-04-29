using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class SubjectGroupService : ISubjectGroupService
    {
        private readonly SqlHelper _sqlHelper;

        public SubjectGroupService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<MstSubjectGroupViewModel> GetAll(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<MstSubjectGroupViewModel>();
            var parameters = new[] 
            { 
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted) 
            };
            var dt = _sqlHelper.ExecuteQuery("sp_SubjectGroup_GetAll", parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        public MstSubjectGroupViewModel? GetByID(int id)
        {
            var parameters = new[] { new SqlParameter("@SubjectGroupID", id) };
            var dt = _sqlHelper.ExecuteQuery("sp_SubjectGroup_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        public (bool success, string message) Upsert(MstSubjectGroupUpsertRequest request, int companyId, int sessionId, int userId)
        {
            try
            {
                string sectionIdsStr = request.SectionIds != null ? string.Join(",", request.SectionIds) : "";
                string subjectIdsStr = request.SubjectIds != null ? string.Join(",", request.SubjectIds) : "";
                
                var parameters = new[]
                {
                    new SqlParameter("@SubjectGroupID", request.SubjectGroupID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@Name", request.Name),
                    new SqlParameter("@ClassID", request.ClassID),
                    new SqlParameter("@Description", (object)request.Description ?? DBNull.Value),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@SectionIds", sectionIdsStr),
                    new SqlParameter("@SubjectIds", subjectIdsStr)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_SubjectGroup_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) Delete(int id, int userId)
        {
            try
            {
                var parameters = new[] { new SqlParameter("@SubjectGroupID", id), new SqlParameter("@UserId", userId) };
                var dt = _sqlHelper.ExecuteQuery("sp_SubjectGroup_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) ToggleStatus(int id, bool isActive, int userId)
        {
            try
            {
                var parameters = new[] { new SqlParameter("@SubjectGroupID", id), new SqlParameter("@IsActive", isActive), new SqlParameter("@UserId", userId) };
                var dt = _sqlHelper.ExecuteQuery("sp_SubjectGroup_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private MstSubjectGroupViewModel MapRowToViewModel(DataRow row)
        {
            var model = new MstSubjectGroupViewModel
            {
                SubjectGroupID = Convert.ToInt32(row["SubjectGroupID"]),
                CompanyID = Convert.ToInt32(row["CompanyID"]),
                SessionID = Convert.ToInt32(row["SessionID"]),
                Name = row["Name"].ToString() ?? "",
                ClassID = Convert.ToInt32(row["ClassID"]),
                Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };

            if (row.Table.Columns.Contains("ClassName")) model.ClassName = row["ClassName"].ToString() ?? "";
            if (row.Table.Columns.Contains("SectionNames")) model.SectionNames = row["SectionNames"].ToString() ?? "";
            if (row.Table.Columns.Contains("SubjectNames")) model.SubjectNames = row["SubjectNames"].ToString() ?? "";

            if (row.Table.Columns.Contains("SectionIds"))
            {
                string ids = row["SectionIds"].ToString() ?? "";
                if (!string.IsNullOrEmpty(ids)) model.SectionIds = ids.Split(',').Select(int.Parse).ToList();
            }

            if (row.Table.Columns.Contains("SubjectIds"))
            {
                string ids = row["SubjectIds"].ToString() ?? "";
                if (!string.IsNullOrEmpty(ids)) model.SubjectIds = ids.Split(',').Select(int.Parse).ToList();
            }

            return model;
        }
    }
}
