using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class AccountEntryService : IAccountEntryService
    {
        private readonly SqlHelper _db;
        public AccountEntryService(SqlHelper db) => _db = db;

        public List<AccountEntryViewModel> GetAllAccountEntries(int companyId, int sessionId, string entryType, bool includeDeleted = false)
        {
            var list = new List<AccountEntryViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@EntryType", entryType),
                    new SqlParameter("@IncludeDeleted", includeDeleted)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_Trn_AccountEntry_GetAll", p).Rows)
                    list.Add(MapAccountEntry(row));
            }
            catch (Exception) { /* Log here if needed */ }
            return list;
        }

        public List<AccountEntryViewModel> SearchAccountEntries(int companyId, int sessionId, string entryType, string searchType, string? dateFrom, string? dateTo)
        {
            var list = new List<AccountEntryViewModel>();
            try
            {
                DateTime? dFrom = null;
                DateTime? dTo = null;
                if (!string.IsNullOrEmpty(dateFrom)) dFrom = DateTime.ParseExact(dateFrom, "dd-MM-yyyy", null);
                if (!string.IsNullOrEmpty(dateTo)) dTo = DateTime.ParseExact(dateTo, "dd-MM-yyyy", null);

                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@EntryType", entryType),
                    new SqlParameter("@SearchType", searchType),
                    new SqlParameter("@DateFrom", (object?)dFrom ?? DBNull.Value),
                    new SqlParameter("@DateTo", (object?)dTo ?? DBNull.Value)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_Trn_AccountEntry_Search", p).Rows)
                    list.Add(MapAccountEntry(row));
            }
            catch (Exception) { /* Log here if needed */ }
            return list;
        }

        public AccountEntryViewModel? GetAccountEntryByID(int id)
        {
            var p = new[] { new SqlParameter("@AccountEntryID", id) };
            var dt = _db.ExecuteQuery("sp_Trn_AccountEntry_GetByID", p);
            return dt.Rows.Count == 0 ? null : MapAccountEntry(dt.Rows[0]);
        }

        public (bool Success, string Message) UpsertAccountEntry(AccountEntryUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@AccountEntryID", req.AccountEntryID),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@AccountHeadID", req.AccountHeadID),
                    new SqlParameter("@EntryType", req.EntryType),
                    new SqlParameter("@Name", req.Name),
                    new SqlParameter("@InvoiceNo", (object?)req.InvoiceNo ?? DBNull.Value),
                    new SqlParameter("@Date", req.Date),
                    new SqlParameter("@Amount", req.Amount),
                    new SqlParameter("@AttachDoc", (object?)req.AttachDoc ?? DBNull.Value),
                    new SqlParameter("@AttachDocType", (object?)req.AttachDocType ?? DBNull.Value),
                    new SqlParameter("@AttachDocName", (object?)req.AttachDocName ?? DBNull.Value),
                    new SqlParameter("@Description", (object?)req.Description ?? DBNull.Value),
                    new SqlParameter("@IsActive", req.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _db.ExecuteQuery("sp_Trn_AccountEntry_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteAccountEntry(int id, int userId)
        {
            try
            {
                var p = new[] { new SqlParameter("@AccountEntryID", id), new SqlParameter("@UserId", userId) };
                var dt = _db.ExecuteQuery("sp_Trn_AccountEntry_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) ToggleAccountEntryStatus(int id, bool isActive, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@AccountEntryID", id),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _db.ExecuteQuery("sp_Trn_AccountEntry_ToggleStatus", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private static AccountEntryViewModel MapAccountEntry(DataRow r) => new()
        {
            AccountEntryID = Convert.ToInt32(r["AccountEntryID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            AccountHeadID = Convert.ToInt32(r["AccountHeadID"]),
            HeadName = r.Table.Columns.Contains("HeadName") ? r["HeadName"].ToString() : null,
            EntryType = r["EntryType"].ToString()!,
            Name = r["Name"].ToString()!,
            InvoiceNo = r["InvoiceNo"] == DBNull.Value ? null : r["InvoiceNo"].ToString(),
            Date = Convert.ToDateTime(r["Date"]),
            Amount = Convert.ToDecimal(r["Amount"]),
            AttachDoc = r["AttachDoc"] == DBNull.Value ? null : (byte[])r["AttachDoc"],
            AttachDocType = r["AttachDocType"] == DBNull.Value ? null : r["AttachDocType"].ToString(),
            AttachDocName = r["AttachDocName"] == DBNull.Value ? null : r["AttachDocName"].ToString(),
            Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
            IsActive = Convert.ToBoolean(r["IsActive"]),
            IsDelete = Convert.ToBoolean(r["IsDelete"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"]),
            CreatedBy = Convert.ToInt32(r["CreatedBy"])
        };
    }
}
