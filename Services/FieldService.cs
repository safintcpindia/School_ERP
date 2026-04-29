using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class FieldService : IFieldService
    {
        private readonly SqlHelper _db;
        public FieldService(SqlHelper db) => _db = db;

        public List<FieldModel> GetAllFields(int companyId, int sessionId, bool? isSystemField = null, string belongsTo = null)
        {
            var list = new List<FieldModel>();
            var p = new[] {
                new SqlParameter("@CompanyId", companyId),
                new SqlParameter("@SessionId", sessionId),
                new SqlParameter("@IsSystemField", (object?)isSystemField ?? DBNull.Value),
                new SqlParameter("@BelongsTo", (object?)belongsTo ?? DBNull.Value)
            };
            foreach (DataRow row in _db.ExecuteQuery("sp_Mst_Fields_GetAll", p).Rows)
                list.Add(MapField(row));
            return list;
        }

        public FieldModel GetFieldByID(int id)
        {
            var p = new[] { new SqlParameter("@FieldId", id) };
            var dt = _db.ExecuteQuery("sp_Mst_Fields_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapField(dt.Rows[0]);
        }

        public (bool Success, string Message) UpsertField(FieldViewModel model, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@FieldId", model.FieldId),
                    new SqlParameter("@BelongsTo", model.BelongsTo),
                    new SqlParameter("@FieldName", model.FieldName),
                    new SqlParameter("@FieldType", model.FieldType),
                    new SqlParameter("@FieldValues", (object?)model.FieldValues ?? DBNull.Value),
                    new SqlParameter("@IsSystemField", model.IsSystemField),
                    new SqlParameter("@IsRequired", model.IsRequired),
                    new SqlParameter("@IsActive", model.IsActive),
                    new SqlParameter("@DisplayOrder", model.DisplayOrder),
                    new SqlParameter("@GridColumn", model.GridColumn),
                    new SqlParameter("@OnTable", model.OnTable),
                    new SqlParameter("@CompanyId", model.CompanyID),
                    new SqlParameter("@SessionId", model.SessionID),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_Fields_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteField(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@FieldId", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_Mst_Fields_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) ToggleFieldStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@FieldId", id), new SqlParameter("@IsActive", isActive), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_Mst_Fields_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public List<IDAutoGenSettings> GetIDAutoGenSettings(int companyId, int sessionId)
        {
            var list = new List<IDAutoGenSettings>();
            var p = new[] {
                new SqlParameter("@CompanyId", companyId),
                new SqlParameter("@SessionId", sessionId)
            };
            var dt = _db.ExecuteQuery("sp_Settings_IDAutoGen_Get", p);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new IDAutoGenSettings
                {
                    ConfigID = Convert.ToInt32(row["ConfigID"]),
                    EntityType = row["EntityType"].ToString()!,
                    IsEnabled = Convert.ToBoolean(row["IsEnabled"]),
                    Prefix = row["Prefix"].ToString(),
                    DigitCount = Convert.ToInt32(row["DigitCount"]),
                    StartNo = Convert.ToInt32(row["StartNo"]),
                    FieldsToInclude = row["FieldsToInclude"].ToString(),
                    CompanyID = Convert.ToInt32(row["CompanyId"]),
                    SessionID = Convert.ToInt32(row["SessionId"]),
                    UpdatedAt = row["ModifiedOn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ModifiedOn"])
                });
            }
            return list;
        }

        public (bool Success, string Message) SaveIDAutoGenSettings(IDAutoGenRequest request, int userId)
        {
            try
            {
                var fields = request.FieldsToInclude != null ? string.Join(",", request.FieldsToInclude) : "";
                var p = new[] {
                    new SqlParameter("@EntityType", request.EntityType),
                    new SqlParameter("@IsEnabled", request.IsEnabled),
                    new SqlParameter("@Prefix", (object?)request.Prefix ?? DBNull.Value),
                    new SqlParameter("@DigitCount", request.DigitCount),
                    new SqlParameter("@StartNo", request.StartNo),
                    new SqlParameter("@FieldsToInclude", fields),
                    new SqlParameter("@CompanyId", request.CompanyID),
                    new SqlParameter("@SessionId", request.SessionID),
                    new SqlParameter("@UserId", userId)
                };
                _db.ExecuteNonQuery("sp_Settings_IDAutoGen_Upsert", p);
                return (true, "Settings saved successfully.");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static FieldModel MapField(DataRow r) => new()
        {
            FieldId = Convert.ToInt32(r["FieldId"]),
            BelongsTo = r["BelongsTo"].ToString()!,
            FieldName = r["FieldName"].ToString()!,
            FieldType = r["FieldType"].ToString()!,
            FieldValues = r["FieldValues"] == DBNull.Value ? null : r["FieldValues"].ToString(),
            IsSystemField = Convert.ToBoolean(r["IsSystemField"]),
            IsRequired = Convert.ToBoolean(r["IsRequired"]),
            IsActive = Convert.ToBoolean(r["IsActive"]),
            DisplayOrder = Convert.ToInt32(r["DisplayOrder"]),
            GridColumn = r.Table.Columns.Contains("GridColumn") ? Convert.ToInt32(r["GridColumn"]) : 12,
            OnTable = r.Table.Columns.Contains("OnTable") && Convert.ToBoolean(r["OnTable"]),
            CompanyID = r.Table.Columns.Contains("CompanyId") && r["CompanyId"] != DBNull.Value ? Convert.ToInt32(r["CompanyId"]) : 0,
            SessionID = r.Table.Columns.Contains("SessionId") && r["SessionId"] != DBNull.Value ? Convert.ToInt32(r["SessionId"]) : 0,
            CreatedOn = Convert.ToDateTime(r["CreatedOn"])
        };
    }
}
