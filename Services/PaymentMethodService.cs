using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This class provides business logic and data access services for PaymentMethodService.
    /// </summary>
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly SqlHelper _sqlHelper;

        public PaymentMethodService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<MstPaymentMethodViewModel> GetAllPaymentMethods(bool includeDeleted = false)
        {
            var list = new List<MstPaymentMethodViewModel>();
            var parameters = new[] { new SqlParameter("@IncludeDeleted", includeDeleted) };
            var dt = _sqlHelper.ExecuteQuery("sp_PaymentMethods_GetAll", parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        public MstPaymentMethodViewModel? GetPaymentMethodById(int paymentId)
        {
            var parameters = new[] { new SqlParameter("@PaymentId", paymentId) };
            var dt = _sqlHelper.ExecuteQuery("sp_PaymentMethods_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        /// <summary>
        /// Upserts payment configurations securely directly firing the 'sp_PaymentMethods_Upsert' layer.
        /// </summary>
        public (bool success, string message) UpsertPaymentMethod(MstPaymentMethodUpsertRequest request, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@PaymentId", request.PaymentId),
                    new SqlParameter("@PaymentKey", request.PaymentKey),
                    new SqlParameter("@PaymentSecret", request.PaymentSecret),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_PaymentMethods_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) DeletePaymentMethod(int paymentId, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@PaymentId", paymentId),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_PaymentMethods_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool success, string message) TogglePaymentMethodStatus(int paymentId, bool isActive, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@PaymentId", paymentId),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_PaymentMethods_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        private MstPaymentMethodViewModel MapRowToViewModel(DataRow row)
        {
            return new MstPaymentMethodViewModel
            {
                PaymentId = Convert.ToInt32(row["PaymentId"]),
                PaymentKey = row["PaymentKey"].ToString() ?? "",
                PaymentSecret = row["PaymentSecret"].ToString() ?? "",
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                ModifiedOn = row["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedOn"]) : (DateTime?)null,
                ModifiedBy = row["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(row["ModifiedBy"]) : (int?)null
            };
        }
    }
}
