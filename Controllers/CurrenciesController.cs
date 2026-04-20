using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for CurrenciesController.
    /// </summary>
    public class CurrenciesController : Controller
    {
        private readonly ICurrencyClientService _currencyClient;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Currencies";

        public CurrenciesController(ICurrencyClientService currencyClient, IUserMenuPermissionService menuPerm)
        {
            _currencyClient = currencyClient;
            _menuPerm = menuPerm;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _currencyClient.GetAllAsync();
            var model = new MstCurrencyPageViewModel
            {
                Currencies = response.Success ? response.Data : new List<MstCurrencyViewModel>()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrency(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit currencies." });

            var response = await _currencyClient.GetByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MstCurrencyUpsertRequest request)
        {
            var isCreate = request.CurrencyId <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add currencies." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit currencies." });

            var response = await _currencyClient.UpsertAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change currency status." });

            var response = await _currencyClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete currencies." });

            var response = await _currencyClient.DeleteAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
