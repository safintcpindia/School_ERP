using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IMenuService
    {
        List<MenuViewModel> GetAllMenus();
        MenuViewModel? GetMenuById(int menuId);
        (int Result, string Message) UpsertMenu(MenuUpsertRequest request, int userId, int mainAccountId, int sessionId, string ipAddress);
        (int Result, string Message) ToggleMenuStatus(int menuId, bool isActive, int userId, int mainAccountId, int sessionId, string ipAddress);
        (int Result, string Message) DeleteMenu(int menuId, int userId, string ipAddress);
    }
}
