using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Services.Clients;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This controller manages the main dashboard, which gives you a quick overview of the whole system's status.
    /// </summary>
    public class DashboardController : Controller
    {
        private readonly IUtilityClientService _utilityClient;
        private readonly ICompanyClientService _companyClient;

        public DashboardController(IUtilityClientService utilityClient, ICompanyClientService companyClient)
        {
            _utilityClient = utilityClient;
            _companyClient = companyClient;
        }

        /// <summary>
        /// Shows the main dashboard page, providing high-level summaries like the total number of companies registered.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Optional hook for extending dashboard statistics in later modules:
            // e.g., var summary = await _utilityClient.GetDashboardSummaryAsync();
            // return View(summary);

            // For now, we simply load the static dashboard framework layout.
            var companiesResponse = await _companyClient.GetAllAsync();
            ViewBag.totalCompanies = companiesResponse.Data.Count();
            return View();
        }
    }
}
