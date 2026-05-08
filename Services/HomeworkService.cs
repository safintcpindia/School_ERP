using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class HomeworkService : IHomeworkService
    {
        private readonly SqlHelper _sqlHelper;

        public HomeworkService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<HomeworkViewModel> GetAll(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<HomeworkViewModel>();
            var parameters = new[]
            {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            var dt = _sqlHelper.ExecuteQuery("sp_Homework_GetAll", parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        public HomeworkViewModel? GetByID(int id)
        {
            var parameters = new[] { new SqlParameter("@HomeworkID", id) };
            var dt = _sqlHelper.ExecuteQuery("sp_Homework_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        public (bool success, string message) Upsert(HomeworkUpsertRequest request, int companyId, int sessionId, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@HomeworkID", request.HomeworkID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@ClassID", request.ClassID),
                    new SqlParameter("@SectionID", request.SectionID),
                    new SqlParameter("@SubjectGroupID", request.SubjectGroupID),
                    new SqlParameter("@SubjectID", request.SubjectID),
                    new SqlParameter("@HomeworkDate", request.HomeworkDate),
                    new SqlParameter("@SubmissionDate", request.SubmissionDate),
                    new SqlParameter("@MaxMarks", (object)request.MaxMarks ?? DBNull.Value),
                    new SqlParameter("@AttachmentPath", (object)request.AttachmentPath ?? DBNull.Value),
                    new SqlParameter("@Description", (object)request.Description ?? DBNull.Value),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Homework_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) Delete(int id, int userId)
        {
            try
            {
                var parameters = new[] { new SqlParameter("@HomeworkID", id), new SqlParameter("@UserId", userId) };
                var dt = _sqlHelper.ExecuteQuery("sp_Homework_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) ToggleStatus(int id, bool isActive, int userId)
        {
            try
            {
                var parameters = new[] { new SqlParameter("@HomeworkID", id), new SqlParameter("@IsActive", isActive), new SqlParameter("@UserId", userId) };
                var dt = _sqlHelper.ExecuteQuery("sp_Homework_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private HomeworkViewModel MapRowToViewModel(DataRow row)
        {
            var model = new HomeworkViewModel
            {
                HomeworkID = Convert.ToInt32(row["HomeworkID"]),
                CompanyID = Convert.ToInt32(row["CompanyID"]),
                SessionID = Convert.ToInt32(row["SessionID"]),
                ClassID = Convert.ToInt32(row["ClassID"]),
                SectionID = Convert.ToInt32(row["SectionID"]),
                SubjectGroupID = Convert.ToInt32(row["SubjectGroupID"]),
                SubjectID = Convert.ToInt32(row["SubjectID"]),
                HomeworkDate = Convert.ToDateTime(row["HomeworkDate"]),
                SubmissionDate = Convert.ToDateTime(row["SubmissionDate"]),
                EvaluationDate = row["EvaluationDate"] != DBNull.Value ? Convert.ToDateTime(row["EvaluationDate"]) : null,
                MaxMarks = row["MaxMarks"] != DBNull.Value ? Convert.ToDecimal(row["MaxMarks"]) : null,
                AttachmentPath = row["AttachmentPath"] != DBNull.Value ? row["AttachmentPath"].ToString() : null,
                Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };

            if (row.Table.Columns.Contains("ClassName")) model.ClassName = row["ClassName"].ToString() ?? "";
            if (row.Table.Columns.Contains("SectionName")) model.SectionName = row["SectionName"].ToString() ?? "";
            if (row.Table.Columns.Contains("SubjectGroupName")) model.SubjectGroupName = row["SubjectGroupName"].ToString() ?? "";
            if (row.Table.Columns.Contains("SubjectName")) model.SubjectName = row["SubjectName"].ToString() ?? "";
            if (row.Table.Columns.Contains("CreatedByName")) model.CreatedByName = row["CreatedByName"].ToString() ?? "";

            return model;
        }
    }
}
