using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IEmailConfigService
    {
        MstEmailConfigViewModel? GetEmailConfig();
        (bool success, string message) UpsertEmailConfig(MstEmailConfigUpsertRequest request, int userId);
    }
}
