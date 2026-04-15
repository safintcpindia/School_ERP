using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This class provides business logic and data access services for CurrencyService.
    /// </summary>
    public class CurrencyService : ICurrencyService
    {
        private readonly SqlHelper _sqlHelper;

        public CurrencyService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<MstCurrencyViewModel> GetAllCurrencies(bool includeDeleted = false)
        {
            var list = new List<MstCurrencyViewModel>();
            var parameters = new[] { new SqlParameter("@IncludeDeleted", includeDeleted) };
            var dt = _sqlHelper.ExecuteQuery("sp_Currencies_GetAll", parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        public MstCurrencyViewModel? GetCurrencyByID(int currencyId)
        {
            var parameters = new[] { new SqlParameter("@CurrencyId", currencyId) };
            var dt = _sqlHelper.ExecuteQuery("sp_Currencies_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        /// <summary>
        /// Commits physical exchange identifiers or modifies rate factors via native SQL stored procedures.
        /// </summary>
        public (bool success, string message) UpsertCurrency(MstCurrencyUpsertRequest request, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@CurrencyId", request.CurrencyId),
                    new SqlParameter("@CurrencyTitle", request.CurrencyTitle),
                    new SqlParameter("@CurrencyCode", request.CurrencyCode),
                    new SqlParameter("@CurrencySymbol", request.CurrencySymbol),
                    new SqlParameter("@CurrencyConvRate", request.CurrencyConvRate),
                    new SqlParameter("@CurrencyBase", request.CurrencyBase),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Currencies_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) DeleteCurrency(int currencyId, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@CurrencyId", currencyId),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Currencies_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) ToggleCurrencyStatus(int currencyId, bool isActive, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@CurrencyId", currencyId),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Currencies_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private MstCurrencyViewModel MapRowToViewModel(DataRow row)
        {
            return new MstCurrencyViewModel
            {
                CurrencyId = Convert.ToInt32(row["CurrencyId"]),
                CurrencyTitle = row["CurrencyTitle"].ToString() ?? "",
                CurrencyCode = row["CurrencyCode"].ToString() ?? "",
                CurrencySymbol = row["CurrencySymbol"].ToString() ?? "",
                CurrencyConvRate = Convert.ToDecimal(row["CurrencyConvRate"]),
                CurrencyBase = Convert.ToBoolean(row["CurrencyBase"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                ModifiedBy = row["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(row["ModifiedBy"]) : null,
                ModifiedOn = row["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedOn"]) : null
            };
        }
    }
}
