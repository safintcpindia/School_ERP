using System.Threading.Tasks;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// Interface for the authentication service. This defines the rules for logging into the system.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// The instruction for checking if a user's name and password are correct.
        /// </summary>
        Task<(int Success, string Message, UserSessionModel User)> LoginAsync(string username, string password);
    }
}
