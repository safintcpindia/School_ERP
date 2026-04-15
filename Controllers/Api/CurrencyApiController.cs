using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// Master data service linking financial denomination constants to billing mechanisms.
    /// </summary>
    public class CurrencyApiController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyApiController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        /// <summary>
        /// Reads all local transaction currencies structured logically for frontend.
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            var data = _currencyService.GetAllCurrencies(includeDeleted);
            return Ok(ApiResponse<List<MstCurrencyViewModel>>.SuccessResponse(data));
        }

        /// <summary>
        /// Exposes base metadata for a singular exchange denomination configuration.
        /// </summary>
        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _currencyService.GetCurrencyByID(id);
            if (data == null) return NotFound(ApiResponse<MstCurrencyViewModel>.ErrorResponse("Denomination not found"));
            return Ok(ApiResponse<MstCurrencyViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Connects new structural monetary tokens mapping against locale setups.
        /// </summary>
        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] MstCurrencyUpsertRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int userId = GetCurrentUserId();
            var (success, message) = _currencyService.UpsertCurrency(request, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Wipes mapping completely (potentially halting dependent ledger rows on Foreign Keys).
        /// </summary>
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _currencyService.DeleteCurrency(id, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Suspends usage natively across global system contexts softly.
        /// </summary>
        [HttpPost("ToggleStatus")]
        public IActionResult ToggleStatus(int id, bool isActive)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _currencyService.ToggleCurrencyStatus(id, isActive, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Contextually parses incoming bearer identities.
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1; 
        }
    }
}
