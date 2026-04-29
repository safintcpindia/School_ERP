using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class SectionService : ISectionService
    {
        private readonly SqlHelper _sqlHelper;

        public SectionService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<MstSectionViewModel> GetAllSections(int companyId, int sessionId, bool includeDeleted = false)
        {
            var list = new List<MstSectionViewModel>();
            var parameters = new[] 
            { 
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IncludeDeleted", includeDeleted) 
            };
            var dt = _sqlHelper.ExecuteQuery("sp_Sections_GetAll", parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        public List<MstSectionViewModel> GetSectionsByClass(int classId)
        {
            var list = new List<MstSectionViewModel>();
            var parameters = new[] { new SqlParameter("@ClassID", classId) };
            var dt = _sqlHelper.ExecuteQuery("sp_Section_GetByClass", parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        public MstSectionViewModel? GetSectionByID(int sectionId)
        {
            var parameters = new[] { new SqlParameter("@SectionID", sectionId) };
            var dt = _sqlHelper.ExecuteQuery("sp_Sections_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        public (bool success, string message) UpsertSection(MstSectionUpsertRequest request, int companyId, int sessionId, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@SectionID", request.SectionID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@SectionName", request.SectionName),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Sections_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) DeleteSection(int sectionId, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@SectionID", sectionId),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Sections_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) ToggleSectionStatus(int sectionId, bool isActive, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@SectionID", sectionId),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Sections_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private MstSectionViewModel MapRowToViewModel(DataRow row)
        {
            var model = new MstSectionViewModel
            {
                SectionID = Convert.ToInt32(row["SectionID"]),
                SectionName = row["SectionName"].ToString() ?? "",
                IsActive = Convert.ToBoolean(row["IsActive"])
            };

            if (row.Table.Columns.Contains("CompanyID")) model.CompanyID = Convert.ToInt32(row["CompanyID"]);
            if (row.Table.Columns.Contains("SessionID")) model.SessionID = Convert.ToInt32(row["SessionID"]);
            if (row.Table.Columns.Contains("CreatedBy")) model.CreatedBy = Convert.ToInt32(row["CreatedBy"]);
            if (row.Table.Columns.Contains("CreatedOn")) model.CreatedOn = Convert.ToDateTime(row["CreatedOn"]);
            if (row.Table.Columns.Contains("ModifiedBy") && row["ModifiedBy"] != DBNull.Value) model.ModifiedBy = Convert.ToInt32(row["ModifiedBy"]);
            if (row.Table.Columns.Contains("ModifiedOn") && row["ModifiedOn"] != DBNull.Value) model.ModifiedOn = Convert.ToDateTime(row["ModifiedOn"]);

            return model;
        }
    }
}
