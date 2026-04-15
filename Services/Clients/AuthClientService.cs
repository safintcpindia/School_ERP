using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for AuthClientService.
    /// </summary>
    public class AuthClientService : BaseApiClient, IAuthClientService
    {
        public AuthClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<UserSessionModel>> LoginAsync(LoginRequest request)
        {
            return await PostAsync<UserSessionModel>("api/auth/login", request);
        }
    }
}
