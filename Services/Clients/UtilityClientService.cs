using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for UtilityClientService.
    /// </summary>
    public class UtilityClientService : BaseApiClient, IUtilityClientService
    {
        public UtilityClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<bool>> SetLanguageAsync(string language)
        {
            return await PostAsync<bool>($"api/UtilityApi/set-language?language={language}", null);
        }

        public async Task<ApiResponse<object>> GetDashboardSummaryAsync()
        {
            return await GetAsync<object>("api/UtilityApi/dashboard-summary");
        }
    }
}
