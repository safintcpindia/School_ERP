using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services.Clients;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for AuthController.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly IAuthClientService _authClient;

        public AuthController(IAuthClientService authClient)
        {
            _authClient = authClient;
        }

        /// <summary>
        /// Serves the initial Login page view.
        /// Unauthenticated users are redirected here.
        /// </summary>
        /// <returns>The Login Razor View.</returns>
        public IActionResult Login()
        {
            // Simply return the default view without a model on first load.
            return View();
        }

        /// <summary>
        /// Processes the submitted login credentials.
        /// Communicates with AuthClientService for token generation or credential validation.
        /// </summary>
        /// <param name="request">The data payload containing Username and Password.</param>
        /// <returns>Redirects to Dashboard if successful, else re-renders Login with error.</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // Ensure the data annotations on the LoginRequest model are valid before making a network call.
            if (!ModelState.IsValid) return View(request);

            // Execute the backend API or service routine to validate credentials.
            var response = await _authClient.LoginAsync(request);

            // Check if the service returned a successful validation flag.
            if (response.Success)
            {
                // In a real application, the authentication cookie or JWT would be materialized here 
                // e.g. HttpContext.SignInAsync(...)
                
                // Route the successfully authenticated user into the secure system layout.
                return RedirectToAction("Index", "Dashboard");
            }

            // Validation failed. Pass the failure string back to the user interface natively.
            ViewBag.ErrorMessage = response.Message;
            return View(request);
        }

        /// <summary>
        /// Purges the active user session and redirects back to the login perimeter.
        /// </summary>
        /// <returns>Redirect to Login action.</returns>
        public IActionResult Logout()
        {
            // Clear cookies used by this app.
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("CurrentSessionId");

            // Also clear client-side localStorage (token + cached user) via a tiny view.
            return View();
        }
    }
}
