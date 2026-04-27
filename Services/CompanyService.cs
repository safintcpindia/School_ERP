using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This service performs the actual work of managing company information, such as saving, updating, or deleting school records from the database.
    /// </summary>
    public class CompanyService : ICompanyService
    {
        private readonly SqlHelper _sqlHelper;

        public CompanyService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        /// <summary>
        /// Retrieves a complete list of all companies from the database.
        /// </summary>
        public List<MstCompanyViewModel> GetAllCompanies(bool includeDeleted = false)
        {
            var list = new List<MstCompanyViewModel>();
            //var parameters = new[] { new SqlParameter("@IncludeDeleted", includeDeleted) };
            var dt = _sqlHelper.ExecuteQuery("sp_Companies_GetAll",null);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToViewModel(row));
            }
            return list;
        }

        /// <summary>
        /// Looks up a specific company's details based on its unique ID.
        /// </summary>
        public MstCompanyViewModel? GetCompanyByID(int companyId)
        {
            var parameters = new[] { new SqlParameter("@CompanyId", companyId) };
            var dt = _sqlHelper.ExecuteQuery("sp_Companies_GetByID", parameters);
            if (dt.Rows.Count == 0) return null;
            return MapRowToViewModel(dt.Rows[0]);
        }

        /// <summary>
        /// Saves company information. If the company already exists, it updates it; otherwise, it creates a new one.
        /// </summary>
        public (bool success, string message) UpsertCompany(MstCompanyUpsertRequest request, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@CompanyId", request.CompanyId),
                    new SqlParameter("@SchoolName", request.SchoolName),
                    new SqlParameter("@SchoolCode", request.SchoolCode),
                    new SqlParameter("@ParentCompanyId", (object?)request.ParentCompanyId ?? DBNull.Value),
                    new SqlParameter("@Address", (object?)request.Address ?? DBNull.Value),
                    new SqlParameter("@Phone", (object?)request.Phone ?? DBNull.Value),
                    new SqlParameter("@Email", (object?)request.Email ?? DBNull.Value),
                    new SqlParameter("@SessionId", request.SessionId),
                    new SqlParameter("@CurrencyId", request.CurrencyId),
                    new SqlParameter("@SessionStartMonth", (object?)request.SessionStartMonth ?? DBNull.Value),
                    new SqlParameter("@DateFormat", (object?)request.DateFormat ?? DBNull.Value),
                    new SqlParameter("@Timezone", (object?)request.Timezone ?? DBNull.Value),
                    new SqlParameter("@StartDayOfWeek", (object?)request.StartDayOfWeek ?? DBNull.Value),
                    new SqlParameter("@CurrencyFormat", (object?)request.CurrencyFormat ?? DBNull.Value),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Companies_Upsert", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a company's record from the database.
        /// </summary>
        public (bool success, string message) DeleteCompany(int companyId, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@CompanyId", companyId),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Companies_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Updates whether a company is currently active or inactive.
        /// </summary>
        public (bool success, string message) ToggleStatus(int companyId, bool isActive, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@CompanyId", companyId),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserId", userId)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Companies_ToggleStatus", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// A helper tool that converts raw database information into a format that the application can easily display.
        /// </summary>
        private MstCompanyViewModel MapRowToViewModel(DataRow row)
        {
            var model = new MstCompanyViewModel
            {
                CompanyId = Convert.ToInt32(row["CompanyId"]),
                SchoolName = row["SchoolName"].ToString() ?? "",
                SchoolCode = row["SchoolCode"].ToString()??"",
                ParentCompanyId = row["ParentCompanyId"] != DBNull.Value ? Convert.ToInt32(row["ParentCompanyId"]) : null,
                ParentName = row.Table.Columns.Contains("ParentName") && row["ParentName"] != DBNull.Value ? row["ParentName"].ToString() : null,
                Address = row["Address"] != DBNull.Value ? row["Address"].ToString() : null,
                Phone = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : null,
                Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : null,
                SessionId = Convert.ToInt32(row["SessionId"]),
                SessionTitle = row.Table.Columns.Contains("SessionTitle") && row["SessionTitle"] != DBNull.Value ? row["SessionTitle"].ToString() : null,
                CurrencyId = Convert.ToInt32(row["CurrencyId"]),
                CurrencyCode = row.Table.Columns.Contains("CurrencyCode") && row["CurrencyCode"] != DBNull.Value ? row["CurrencyCode"].ToString() : null,
                SessionStartMonth = row["SessionStartMonth"] != DBNull.Value ? row["SessionStartMonth"].ToString() : null,
                DateFormat = row["DateFormat"] != DBNull.Value ? row["DateFormat"].ToString() : null,
                Timezone = row["Timezone"] != DBNull.Value ? row["Timezone"].ToString() : null,
                StartDayOfWeek = row["StartDayOfWeek"] != DBNull.Value ? row["StartDayOfWeek"].ToString() : null,
                CurrencyFormat = row["CurrencyFormat"] != DBNull.Value ? row["CurrencyFormat"].ToString() : null,
                CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"]),
                ModifiedBy = row["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(row["ModifiedBy"]) : null,
                ModifiedOn = row["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedOn"]) : null,
                IsActive = Convert.ToBoolean(row["IsActive"]),
                IsDelete = Convert.ToBoolean(row["IsDelete"])
            };
            return model;
        }
    }
}
