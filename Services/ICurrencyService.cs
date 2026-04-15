using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface ICurrencyService
    {
        List<MstCurrencyViewModel> GetAllCurrencies(bool includeDeleted = false);
        MstCurrencyViewModel? GetCurrencyByID(int currencyId);
        (bool success, string message) UpsertCurrency(MstCurrencyUpsertRequest request, int userId);
        (bool success, string message) DeleteCurrency(int currencyId, int userId);
        (bool success, string message) ToggleCurrencyStatus(int currencyId, bool isActive, int userId);
    }
}
