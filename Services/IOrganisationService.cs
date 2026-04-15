using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IOrganisationService
    {
        List<OrganisationViewModel> GetAllOrganisations(bool includeDeleted = false);
        OrganisationViewModel? GetOrganisationByID(int organisationID);
        (bool success, string message) UpsertOrganisation(OrganisationUpsertRequest request, int userId);
        (bool success, string message) DeleteOrganisation(int organisationID, int userId);
        (bool success, string message) ToggleOrganisationStatus(int organisationID, bool isActive, int userId);
    }
}
