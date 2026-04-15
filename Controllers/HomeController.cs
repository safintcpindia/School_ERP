using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Controllers;

/// <summary>
/// This class handles HTTP routing and API requests for HomeController.
/// Acts as the default fallback route processor for public-facing or root interactions.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Serves the main public-facing landing page of the application, if any.
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Serves the generic Privacy Policy text page.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Global error handling action bound by the exception middleware.
    /// Extracts the trace identifier to display to the user for support ticketing.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Activity.Current?.Id retrieves the active diagnostic correlation ID.
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
