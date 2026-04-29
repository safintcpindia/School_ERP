using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class AccountHeadService : IAccountHeadService
    {
        private readonly SqlHelper _db;
        public AccountHeadService(SqlHelper db) => _db = db;

        public List<AccountHeadViewModel> GetAllAccountHeads(int companyId, int sessionId, string headType, bool includeDeleted = false)
        {
            var list = new List<AccountHeadViewModel>();
            var p = new[] {
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@HeadType", headType),
                new SqlParameter("@IncludeDeleted", includeDeleted)
            };
            foreach (DataRow row in _db.ExecuteQuery("sp_Mst_AccountHead_GetAll", p).Rows)
                list.Add(MapAccountHead(row));
            return list;
        }

        public AccountHeadViewModel? GetAccountHeadByID(int id)
        {
            var p = new[] { new SqlParameter("@AccountHeadID", id) };
            var dt = _db.ExecuteQuery("sp_Mst_AccountHead_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapAccountHead(dt.Rows[0]);
        }

        public (bool Success, string Message) UpsertAccountHead(AccountHeadUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@AccountHeadID", req.AccountHeadID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@HeadName", req.HeadName),
                    new SqlParameter("@HeadDescription", (object?)req.HeadDescription ?? DBNull.Value),
                    new SqlParameter("@HeadType", req.HeadType),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_AccountHead_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteAccountHead(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@AccountHeadID", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_Mst_AccountHead_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) ToggleAccountHeadStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@AccountHeadID", id),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _db.ExecuteQuery("sp_Mst_AccountHead_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static AccountHeadViewModel MapAccountHead(DataRow r) => new()
        {
            AccountHeadID = Convert.ToInt32(r["AccountHeadID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            HeadName = r["HeadName"].ToString()!,
            HeadDescription = r["HeadDescription"] == DBNull.Value ? null : r["HeadDescription"].ToString(),
            HeadType = r["HeadType"].ToString()!,
            IsActive = Convert.ToBoolean(r["IsActive"]),
            IsDelete = Convert.ToBoolean(r["IsDelete"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"]),
            CreatedBy = Convert.ToInt32(r["CreatedBy"]),
            ModifiedOn = r["ModifiedOn"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["ModifiedOn"]),
            ModifiedBy = r["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["ModifiedBy"])
        };
    }
}
