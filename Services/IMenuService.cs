using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This interface defines the rules for managing the application's navigation menus.
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// Gets a list of all menus available in the system.
        /// </summary>
        List<MenuViewModel> GetAllMenus();

        /// <summary>
        /// Finds the details of a specific menu item using its ID.
        /// </summary>
        MenuViewModel? GetMenuById(int menuId);

        /// <summary>
        /// Adds a new menu item or updates an existing one.
        /// </summary>
        (int Result, string Message) UpsertMenu(MenuUpsertRequest request, int userId, int mainAccountId, int sessionId, string ipAddress);

        /// <summary>
        /// Turns a menu item on or off (hides or shows it).
        /// </summary>
        (int Result, string Message) ToggleMenuStatus(int menuId, bool isActive, int userId, int mainAccountId, int sessionId, string ipAddress);

        /// <summary>
        /// Removes a menu item from the system.
        /// </summary>
        (int Result, string Message) DeleteMenu(int menuId, int userId, string ipAddress);

        /// <summary>
        /// Changes the order in which menu items appear (e.g., moving one item above another).
        /// </summary>
        (int Result, string Message) UpdateMenuOrder(List<MenuOrderRequest> orders, int userId, string ipAddress);
    }
}
