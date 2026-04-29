using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly SqlHelper _sqlHelper;

        public SubjectService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<MstSubjectViewModel> GetAllSubjects(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<MstSubjectViewModel>();
            var parameters = new[] 
            { 
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted) 
            };
            var dt = _sqlHelper.ExecuteQuery("sp_Subject_GetAll", parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        public MstSubjectViewModel? GetSubjectByID(int subjectId)
        {
            var parameters = new[] { new SqlParameter("@SubjectID", subjectId) };
            var dt = _sqlHelper.ExecuteQuery("sp_Subject_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        public (bool success, string message) UpsertSubject(MstSubjectUpsertRequest request, int companyId, int sessionId, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@SubjectID", request.SubjectID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@SubjectName", request.SubjectName),
                    new SqlParameter("@SubjectType", (object)request.SubjectType ?? DBNull.Value),
                    new SqlParameter("@SubjectCode", (object)request.SubjectCode ?? DBNull.Value),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Subject_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) DeleteSubject(int subjectId, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@SubjectID", subjectId),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Subject_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) ToggleSubjectStatus(int subjectId, bool isActive, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@SubjectID", subjectId),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Subject_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private MstSubjectViewModel MapRowToViewModel(DataRow row)
        {
            return new MstSubjectViewModel
            {
                SubjectID = Convert.ToInt32(row["SubjectID"]),
                CompanyID = Convert.ToInt32(row["CompanyID"]),
                SessionID = Convert.ToInt32(row["SessionID"]),
                SubjectName = row["SubjectName"].ToString() ?? "",
                SubjectType = row["SubjectType"] != DBNull.Value ? row["SubjectType"].ToString() : null,
                SubjectCode = row["SubjectCode"] != DBNull.Value ? row["SubjectCode"].ToString() : null,
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                ModifiedBy = row["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(row["ModifiedBy"]) : null,
                ModifiedOn = row["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedOn"]) : null
            };
        }
    }
}
