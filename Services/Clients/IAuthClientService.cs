using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface IAuthClientService
    {
        Task<ApiResponse<UserSessionModel>> LoginAsync(LoginRequest request);
    }
}
