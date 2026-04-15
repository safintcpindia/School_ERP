using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface ISmsConfigService
    {
        MstSmsConfigViewModel? GetSmsConfig();
        (bool success, string message) UpsertSmsConfig(MstSmsConfigUpsertRequest request, int userId);
    }
}
