using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services.Clients;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for CurrenciesController.
    /// </summary>
    public class CurrenciesController : Controller
    {
        private readonly ICurrencyClientService _currencyClient;

        public CurrenciesController(ICurrencyClientService currencyClient)
        {
            _currencyClient = currencyClient;
        }

        /// <summary>
        /// Instantiates the root view listing all known currencies.
        /// Extracts records synchronously for the HTML page rendering map.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var response = await _currencyClient.GetAllAsync();
            var model = new MstCurrencyPageViewModel
            {
                Currencies = response.Success ? response.Data : new List<MstCurrencyViewModel>()
            };
            return View(model);
        }

        /// <summary>
        /// Reads a distinct currency configuration by key, enabling the frontend partials to reflect settings.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCurrency(int id)
        {
            var response = await _currencyClient.GetByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

        /// <summary>
        /// Intercepts and formats form submissions targeting currency generation/updating.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MstCurrencyUpsertRequest request)
        {
            var response = await _currencyClient.UpsertAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Changes whether a financial unit is active in UI selection modules.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var response = await _currencyClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Drops a currency from the schema physically or logically depending on service constraints.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _currencyClient.DeleteAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
