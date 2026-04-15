using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Services.Clients;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for DashboardController.
    /// </summary>
    public class DashboardController : Controller
    {
        private readonly IUtilityClientService _utilityClient;

        public DashboardController(IUtilityClientService utilityClient)
        {
            _utilityClient = utilityClient;
        }

        /// <summary>
        /// Serves the default landing page for authenticated users.
        /// Gathers high-level system summaries.
        /// </summary>
        /// <returns>The Dashboard Razor View.</returns>
        public async Task<IActionResult> Index()
        {
            // Optional hook for extending dashboard statistics in later modules:
            // e.g., var summary = await _utilityClient.GetDashboardSummaryAsync();
            // return View(summary);
            
            // For now, we simply load the static dashboard framework layout.
            return View();
        }
    }
}
