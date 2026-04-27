using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This service handles the actual work of managing the navigation menus, such as saving their order, names, and links in the database.
    /// </summary>
    public class MenuService : IMenuService
    {
        private readonly SqlHelper _sqlHelper;

        public MenuService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        /// <summary>
        /// Retrieves a list of all navigation menus from the database.
        /// </summary>
        public List<MenuViewModel> GetAllMenus()
        {
            var menus = new List<MenuViewModel>();
            try
            {
                DataTable dt = _sqlHelper.ExecuteQuery("sp_Menus_GetAll", null!);

                foreach (DataRow dr in dt.Rows)
                {
                    menus.Add(new MenuViewModel
                    {
                        MenuID = dr["MenuID"] != DBNull.Value ? (int)dr["MenuID"] : 0,
                        MenuName = dr["MenuName"]?.ToString() ?? string.Empty,
                        DisplayLabel = dr["DisplayLabel"]?.ToString(),
                        MenuURL = dr["MenuURL"]?.ToString() ?? string.Empty,
                        ParentID = (dr["ParentID"] != DBNull.Value && (int)dr["ParentID"] != 0) ? (int?)dr["ParentID"] : null,
                        ParentMenuName = dr["ParentMenuName"]?.ToString(),
                        DisplayOrder = dr["DisplayOrder"] != DBNull.Value ? (int)dr["DisplayOrder"] : 0,
                        MenuIcon = dr["MenuIcon"]?.ToString(),
                        MenuKey = dr["MenuKey"]?.ToString(),
                        IsActive = dr["IsActive"] != DBNull.Value && (bool)dr["IsActive"],
                        CreatedBy = dr["CreatedBy"] != DBNull.Value ? (int)dr["CreatedBy"] : 0,
                        CreatedOn = dr["CreatedOn"] != DBNull.Value ? (DateTime)dr["CreatedOn"] : DateTime.MinValue,
                        ModifiedBy = dr["ModifiedBy"] != DBNull.Value ? (int?)dr["ModifiedBy"] : null,
                        ModifiedOn = dr["ModifiedOn"] != DBNull.Value ? (DateTime?)dr["ModifiedOn"] : null
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetAllMenus: " + ex.Message);
            }

            return menus;
        }

        /// <summary>
        /// Looks up the details of a specific menu item from the database.
        /// </summary>
        public MenuViewModel? GetMenuById(int menuId)
        {
            try
            {
                var parameters = new[] { new SqlParameter("@MenuID", menuId) };
                DataTable dt = _sqlHelper.ExecuteQuery("sp_Menus_GetByID", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    return new MenuViewModel
                    {
                        MenuID = dr["MenuID"] != DBNull.Value ? (int)dr["MenuID"] : 0,
                        MenuName = dr["MenuName"]?.ToString() ?? string.Empty,
                        DisplayLabel = dr["DisplayLabel"]?.ToString(),
                        MenuURL = dr["MenuURL"]?.ToString() ?? string.Empty,
                        ParentID = (dr["ParentID"] != DBNull.Value && (int)dr["ParentID"] != 0) ? (int?)dr["ParentID"] : null,
                        ParentMenuName = dr["ParentMenuName"]?.ToString(),
                        DisplayOrder = dr["DisplayOrder"] != DBNull.Value ? (int)dr["DisplayOrder"] : 0,
                        MenuIcon = dr["MenuIcon"]?.ToString(),
                        MenuKey = dr["MenuKey"]?.ToString(),
                        IsActive = dr["IsActive"] != DBNull.Value && (bool)dr["IsActive"],
                        CreatedBy = dr["CreatedBy"] != DBNull.Value ? (int)dr["CreatedBy"] : 0,
                        CreatedOn = dr["CreatedOn"] != DBNull.Value ? (DateTime)dr["CreatedOn"] : DateTime.MinValue,
                        ModifiedBy = dr["ModifiedBy"] != DBNull.Value ? (int?)dr["ModifiedBy"] : null,
                        ModifiedOn = dr["ModifiedOn"] != DBNull.Value ? (DateTime?)dr["ModifiedOn"] : null
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetMenuById: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Saves or updates a menu item's information, including its name, icon, and link.
        /// </summary>
        public (int Result, string Message) UpsertMenu(MenuUpsertRequest request, int userId, int mainAccountId, int sessionId, string ipAddress)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@MenuID", request.MenuID),
                    new SqlParameter("@MenuName", request.MenuName),
                    new SqlParameter("@DisplayLabel", request.DisplayLabel ?? (object)DBNull.Value),
                    new SqlParameter("@MenuURL", request.MenuURL),
                    new SqlParameter("@ParentID", request.ParentID ?? (object)DBNull.Value),
                    new SqlParameter("@DisplayOrder", request.DisplayOrder),
                    new SqlParameter("@MenuIcon", request.MenuIcon ?? (object)DBNull.Value),
                    new SqlParameter("@MenuKey", request.MenuKey ?? (object)DBNull.Value),
                    new SqlParameter("@IsActive", request.IsActive),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@MainAccountId", mainAccountId),
                    new SqlParameter("@SessionId", sessionId),
                    new SqlParameter("@IPAddress", ipAddress)
                };

                DataTable dt = _sqlHelper.ExecuteQuery("sp_Menus_Upsert", parameters);
                if (dt.Rows.Count > 0)
                {
                    int result = (int)dt.Rows[0]["Result"];
                    string message = dt.Rows[0]["Message"].ToString() ?? "Unknown error";
                    return (result, message);
                }
            }
            catch (Exception ex)
            {
                return (-1, "Database Error: " + ex.Message);
            }

            return (-1, "No response from database");
        }

        /// <summary>
        /// Updates whether a menu item should be shown or hidden.
        /// </summary>
        public (int Result, string Message) ToggleMenuStatus(int menuId, bool isActive, int userId, int mainAccountId, int sessionId, string ipAddress)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@MenuID", menuId),
                    new SqlParameter("@IsActive", isActive),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@MainAccountId", mainAccountId),
                    new SqlParameter("@SessionId", sessionId),
                    new SqlParameter("@IPAddress", ipAddress)
                };

                DataTable dt = _sqlHelper.ExecuteQuery("sp_Menus_ToggleStatus", parameters);
                if (dt.Rows.Count > 0)
                {
                    int result = (int)dt.Rows[0]["Result"];
                    string message = dt.Rows[0]["Message"].ToString() ?? "Unknown error";
                    return (result, message);
                }
            }
            catch (Exception ex)
            {
                return (-1, "Database Error: " + ex.Message);
            }

            return (-1, "No response from database");
        }

        /// <summary>
        /// Deletes a menu item from the database.
        /// </summary>
        public (int Result, string Message) DeleteMenu(int menuId, int userId, string ipAddress)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@MenuID", menuId),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@IPAddress", ipAddress)
                };

                DataTable dt = _sqlHelper.ExecuteQuery("sp_Menus_Delete", parameters);
                if (dt.Rows.Count > 0)
                {
                    int result = (int)dt.Rows[0]["Result"];
                    string message = dt.Rows[0]["Message"].ToString() ?? "Unknown error";
                    return (result, message);
                }
            }
            catch (Exception ex)
            {
                return (-1, "Database Error: " + ex.Message);
            }

            return (-1, "No response from database");
        }

        /// <summary>
        /// Updates the sequence in which menu items are displayed on the screen.
        /// </summary>
        public (int Result, string Message) UpdateMenuOrder(List<MenuOrderRequest> orders, int userId, string ipAddress)
        {
            int successCount = 0;
            try
            {
                foreach (var order in orders)
                {
                    var parameters = new[]
                    {
                        new SqlParameter("@MenuID", order.MenuID),
                        new SqlParameter("@DisplayOrder", order.DisplayOrder),
                        new SqlParameter("@UserID", userId),
                        new SqlParameter("@IPAddress", ipAddress)
                    };
                    _sqlHelper.ExecuteNonQuery("sp_Menus_UpdateOrder", parameters);
                    successCount++;
                }

                if (successCount == orders.Count)
                {
                    return (1, "Menu order updated successfully");
                }
                else
                {
                    return (0, $"Updated {successCount} of {orders.Count} items");
                }
            }
            catch (Exception ex)
            {
                return (-1, $"Error updating menu order: {ex.Message}");
            }
        }
    }
}
