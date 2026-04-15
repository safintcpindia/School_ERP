using System.Threading.Tasks;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface IUtilityClientService
    {
        Task<ApiResponse<bool>> SetLanguageAsync(string language);
        Task<ApiResponse<object>> GetDashboardSummaryAsync();
    }
}
