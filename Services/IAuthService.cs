using System.Threading.Tasks;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IAuthService
    {
        Task<(int Success, string Message, UserSessionModel User)> LoginAsync(string username, string password);
    }
}
