using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This class provides business logic and data access services for MenuService.
    /// </summary>
    public class MenuService : IMenuService
    {
        private readonly SqlHelper _sqlHelper;

        public MenuService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<MenuViewModel> GetAllMenus()
        {
            var menus = new List<MenuViewModel>();
            DataTable dt = _sqlHelper.ExecuteQuery("sp_Menus_GetAll", null!);

            foreach (DataRow dr in dt.Rows)
            {
                menus.Add(new MenuViewModel
                {
                    MenuID = (int)dr["MenuID"],
                    MenuName = dr["MenuName"]?.ToString() ?? "",
                    DisplayLabel = dr["DisplayLabel"]?.ToString(),
                    MenuURL = dr["MenuURL"]?.ToString() ?? "",
                    ParentID = dr["ParentID"] != DBNull.Value ? (int)dr["ParentID"] : null,
                    ParentMenuName = dr["ParentMenuName"]?.ToString(),
                    DisplayOrder = (int)dr["DisplayOrder"],
                    MenuIcon = dr["MenuIcon"]?.ToString(),
                    IsActive = (bool)dr["IsActive"],
                    CreatedBy = (int)dr["CreatedBy"],
                    CreatedOn = (DateTime)dr["CreatedOn"],
                    ModifiedBy = dr["ModifiedBy"] != DBNull.Value ? (int)dr["ModifiedBy"] : null,
                    ModifiedOn = dr["ModifiedOn"] != DBNull.Value ? (DateTime)dr["ModifiedOn"] : null
                });
            }

            return menus;
        }

        public MenuViewModel? GetMenuById(int menuId)
        {
            var parameters = new[] { new SqlParameter("@MenuID", menuId) };
            DataTable dt = _sqlHelper.ExecuteQuery("sp_Menus_GetByID", parameters);

            if (dt.Rows.Count == 0) return null;

            var dr = dt.Rows[0];
            return new MenuViewModel
            {
                MenuID = (int)dr["MenuID"],
                MenuName = dr["MenuName"]?.ToString() ?? "",
                DisplayLabel = dr["DisplayLabel"]?.ToString(),
                MenuURL = dr["MenuURL"]?.ToString() ?? "",
                ParentID = dr["ParentID"] != DBNull.Value ? (int)dr["ParentID"] : null,
                DisplayOrder = (int)dr["DisplayOrder"],
                MenuIcon = dr["MenuIcon"]?.ToString(),
                IsActive = (bool)dr["IsActive"]
            };
        }

        /// <summary>
        /// Connects the dynamic UI element request to the 'sp_Menus_Upsert' backend hook.
        /// Updates sorting layers and routing strings globally.
        /// </summary>
        public (int Result, string Message) UpsertMenu(MenuUpsertRequest request, int userId, int mainAccountId, int sessionId, string ipAddress)
        {
            var parameters = new[]
            {
                new SqlParameter("@MenuID", request.MenuID),
                new SqlParameter("@MenuName", request.MenuName),
                new SqlParameter("@DisplayLabel", (object?)request.DisplayLabel ?? DBNull.Value),
                new SqlParameter("@MenuURL", request.MenuURL),
                new SqlParameter("@ParentID", (object?)request.ParentID ?? DBNull.Value),
                new SqlParameter("@DisplayOrder", request.DisplayOrder),
                new SqlParameter("@MenuIcon", (object?)request.MenuIcon ?? DBNull.Value),
                new SqlParameter("@IsActive", request.IsActive),
                new SqlParameter("@UserID", userId),
                new SqlParameter("@MainAccountID", mainAccountId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IPAddress", ipAddress)
            };

            DataTable dt = _sqlHelper.ExecuteQuery("sp_Menus_Upsert", parameters);
            if (dt.Rows.Count > 0)
            {
                return ((int)dt.Rows[0]["Result"], dt.Rows[0]["Message"]?.ToString() ?? "");
            }

            return (-99, "Unknown error");
        }

        public (int Result, string Message) ToggleMenuStatus(int menuId, bool isActive, int userId, int mainAccountId, int sessionId, string ipAddress)
        {
            var parameters = new[]
            {
                new SqlParameter("@MenuID", menuId),
                new SqlParameter("@IsActive", isActive),
                new SqlParameter("@UserID", userId),
                new SqlParameter("@MainAccountID", mainAccountId),
                new SqlParameter("@SessionID", sessionId),
                new SqlParameter("@IPAddress", ipAddress)
            };

            DataTable dt = _sqlHelper.ExecuteQuery("sp_Menus_ToggleStatus", parameters);
            if (dt.Rows.Count > 0)
            {
                return ((int)dt.Rows[0]["Result"], dt.Rows[0]["Message"]?.ToString() ?? "");
            }

            return (-99, "Unknown error");
        }

        public (int Result, string Message) DeleteMenu(int menuId, int userId, string ipAddress)
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
                return ((int)dt.Rows[0]["Result"], dt.Rows[0]["Message"]?.ToString() ?? "");
            }

            return (-99, "Unknown error");
        }
    }
}
