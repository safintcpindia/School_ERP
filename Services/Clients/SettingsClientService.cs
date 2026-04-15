using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for SettingsClientService.
    /// </summary>
    public class SettingsClientService : BaseApiClient, ISettingsClientService
    {
        public SettingsClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<Dictionary<string, string>>> GetTranslationsAsync(string language)
        {
            return await GetAsync<Dictionary<string, string>>($"api/SettingsApi/translations/{language}");
        }

        public async Task<ApiResponse<bool>> UpdateTranslationAsync(TranslationUpdateModel model)
        {
            return await PostAsync<bool>("api/SettingsApi/translations/update", model);
        }
    }
}
