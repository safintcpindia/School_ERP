using SchoolERP.Net.Models;
using System.Collections.Generic;

namespace SchoolERP.Net.Services
{
    public interface IPaymentMethodService
    {
        List<MstPaymentMethodViewModel> GetAllPaymentMethods(bool includeDeleted = false);
        MstPaymentMethodViewModel? GetPaymentMethodById(int paymentId);
        (bool success, string message) UpsertPaymentMethod(MstPaymentMethodUpsertRequest request, int userId);
        (bool success, string message) DeletePaymentMethod(int paymentId, int userId);
        (bool success, string message) TogglePaymentMethodStatus(int paymentId, bool isActive, int userId);
    }
}
