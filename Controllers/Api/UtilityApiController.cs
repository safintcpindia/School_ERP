using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This class handles HTTP routing and API requests for UtilityApiController.
    /// </summary>
    public class UtilityApiController : ControllerBase
    {
        private readonly ILocalizationService _localizationService;

        public UtilityApiController(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        /// <summary>
        /// Fires a state mutation changing the active session's rendering locale constraint.
        /// </summary>
        [HttpPost("set-language")]
        public IActionResult SetLanguage([FromQuery] string language)
        {
            _localizationService.SetLanguage(language);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Language set successfully"));
        }

        /// <summary>
        /// Computes quick aggregate counts mapped to the master analytics dashboard context.
        /// </summary>
        [HttpGet("dashboard-summary")]
        public IActionResult GetDashboardSummary()
        {
            // Placeholder for dashboard stats
            return Ok(ApiResponse<object>.SuccessResponse(new { totalUsers = 10, uptime = "99.9%" }));
        }
    }
}
