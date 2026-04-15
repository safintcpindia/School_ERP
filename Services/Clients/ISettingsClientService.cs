using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface ISettingsClientService
    {
        Task<ApiResponse<Dictionary<string, string>>> GetTranslationsAsync(string language);
        Task<ApiResponse<bool>> UpdateTranslationAsync(TranslationUpdateModel model);
    }
}
