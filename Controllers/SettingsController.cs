using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for SettingsController.
    /// </summary>
    public class SettingsController : Controller
    {
        private readonly ISettingsClientService _settingsClient;
        private readonly IRoleClientService _roleClient;
        private readonly IUserClientService _userClient;
        private readonly IEmailConfigClientService _emailClient;
        private readonly ISmsConfigClientService _smsClient;
        private readonly IPaymentMethodClientService _paymentClient;
        private readonly ICompanyClientService _companyClient;
        private readonly ISessionClientService _sessionClient;
        private readonly ICurrencyClientService _currencyClient;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string SettingsMenuPath = "/Settings";

        public SettingsController(
            ISettingsClientService settingsClient, 
            IRoleClientService roleClient, 
            IUserClientService userClient,
            IEmailConfigClientService emailClient,
            ISmsConfigClientService smsClient,
            IPaymentMethodClientService paymentClient,
            ICompanyClientService companyClient,
            ISessionClientService sessionClient,
            ICurrencyClientService currencyClient,
            IUserMenuPermissionService menuPerm)
        {
            _settingsClient = settingsClient;
            _roleClient = roleClient;
            _userClient = userClient;
            _emailClient = emailClient;
            _smsClient = smsClient;
            _paymentClient = paymentClient;
            _companyClient = companyClient;
            _sessionClient = sessionClient;
            _currencyClient = currencyClient;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Serves the high-level General Settings layout interface.
        /// </summary>
        public IActionResult CompanyMaster()
        {
            ViewData["Title"] = "Company Master";
            return View();
        }

        /// <summary>
        /// Serves the Roles and Permissions master matrix interface.
        /// Loads all static system roles on page-load to construct the master data table.
        /// </summary>
        public async Task<IActionResult> Roles()
        {
            ViewData["Title"] = "Roles & Permission";
            
            // Invoke the backend Role Service to fetch all available roles
            var rolesResponse = await _roleClient.GetAllRolesAsync();
            
            // Map the service response to the ViewModel, providing an empty list fallback if the database call fails
            var model = new RolesPageViewModel
            {
                Roles = rolesResponse.Success
                    ? rolesResponse.Data.Select(r => new RoleViewModel
                    {
                        RoleID = r.RoleID,
                        RoleName = r.RoleName,
                        Description = r.RoleDesc ?? "",
                        IsActive = r.IsActive
                    }).ToList()
                    : new List<RoleViewModel>()
            };
            return View(model);
        }

        /// <summary>
        /// Retrieves the hierarchical permission constraints linked to a specific RoleID.
        /// Consumed by AJAX to build the dual-modal checkbox tree.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPermissions(int roleId)
        {
            if (!_menuPerm.Has(User, SettingsMenuPath, "View"))
                return Json(new { success = false, message = "You do not have permission to view role permissions.", permissions = (object?)null });

            var response = await _roleClient.GetPermissionsAsync(roleId);
            return Json(new { success = response.Success, permissions = response.Data, message = response.Message });
        }

        /// <summary>
        /// Handles AJAX form dispatches for creating or modifying a structural role.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveRole([FromBody] RoleUpsertRequest request)
        {
            var isCreate = request.RoleID <= 0;
            if (isCreate && !_menuPerm.Has(User, SettingsMenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add roles." });
            if (!isCreate && !_menuPerm.Has(User, SettingsMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit roles." });

            // Transform the simplified incoming request into the strict Upsert pipeline request
            var serviceRequest = new MstRoleUpsertRequest
            {
                RoleID = request.RoleID,
                RoleName = request.RoleName,
                RoleDesc = request.Description,
                IsActive = true // Force active status on new creates from Settings Module wrapper
            };

            var response = await _roleClient.SaveRoleAsync(serviceRequest);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Submits the checked tree boxes back to the persistence layer for a bulk map override.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveRolePermissions([FromBody] MstRolePermissionSaveRequest request)
        {
            if (!_menuPerm.Has(User, SettingsMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to update role permissions." });

            var response = await _roleClient.SavePermissionsAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Academic Session period configuration view.
        /// </summary>
        public IActionResult Sessions()
        {
            ViewData["Title"] = "Sessions Setting";
            return View();
        }

        /// <summary>
        /// Global financial currency configuration view.
        /// </summary>
        public IActionResult Currencies()
        {
            ViewData["Title"] = "Currencies";
            return View();
        }

        /// <summary>
        /// Renders the SMTP gateway setup view and binds the active mail server constraints if they exist.
        /// </summary>
        public async Task<IActionResult> EmailSettings()
        {
            ViewData["Title"] = "Email Settings";
            var response = await _emailClient.GetEmailConfigAsync();
            var model = (response.Success && response.Data != null) ? response.Data : new MstEmailConfigViewModel();
            return View(model);
        }

        /// <summary>
        /// Mutates the global SMTP credentials or outgoing host profile.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveEmailSettings([FromBody] MstEmailConfigUpsertRequest request)
        {
            if (!_menuPerm.Has(User, SettingsMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change email settings." });

            var response = await _emailClient.UpsertEmailConfigAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Renders the API integration settings for SMS communication gateways.
        /// </summary>
        public async Task<IActionResult> SmsSettings()
        {
            ViewData["Title"] = "SMS Settings";
            var response = await _smsClient.GetSmsConfigAsync();
            var model = (response.Success && response.Data != null) ? response.Data : new MstSmsConfigViewModel();
            return View(model);
        }

        /// <summary>
        /// Modifies the active SMS gateway URL payloads and keys.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveSmsSettings([FromBody] MstSmsConfigUpsertRequest request)
        {
            if (!_menuPerm.Has(User, SettingsMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change SMS settings." });

            var response = await _smsClient.UpsertSmsConfigAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Locales definition page for dynamic module mapping.
        /// </summary>
        public IActionResult Languages()
        {
            ViewData["Title"] = "Languages";
            return View();
        }

        /// <summary>
        /// Deep-dives into a specific locale (e.g. "en_US") to view dictionary JSON overrides.
        /// </summary>
        public async Task<IActionResult> Translations(string id)
        {
            // Fail safe routing to root Languages board
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Languages");
            
            ViewData["Title"] = "Translate " + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(id.Replace("_", " "));
            ViewData["SelectedLanguage"] = id;
            
            // Extracts all JSON localized string translations from db/file cache
            var response = await _settingsClient.GetTranslationsAsync(id);
            return View(response.Success ? response.Data : new Dictionary<string, string>());
        }

        /// <summary>
        /// Granular REST update for one dictionary phrase translation.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateTranslation([FromBody] TranslationUpdateModel model)
        {
            if (!_menuPerm.Has(User, SettingsMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to update translations." });

            var response = await _settingsClient.UpdateTranslationAsync(model);
            return Json(new { success = response.Success, message = response.Message });
        }

        public IActionResult SidebarMenu()
        {
            return RedirectToAction("Index", "MasterMenu");
        }

        public IActionResult Notifications() => View();
        
        /// <summary>
        /// Lists third party gateway configurations available to the billing module.
        /// </summary>
        public async Task<IActionResult> PaymentMethods()
        {
            ViewData["Title"] = "Payment Methods";
            var response = await _paymentClient.GetAllPaymentMethodsAsync();
            var model = new PaymentMethodsPageViewModel
            {
                PaymentMethods = response.Success ? response.Data : new List<MstPaymentMethodViewModel>()
            };
            return View(model);
        }

        /// <summary>
        /// Inserts or updates a billing gateway instance.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SavePaymentMethod([FromBody] MstPaymentMethodUpsertRequest request)
        {
            var isCreate = request.PaymentId <= 0;
            if (isCreate && !_menuPerm.Has(User, SettingsMenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add payment methods." });
            if (!isCreate && !_menuPerm.Has(User, SettingsMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit payment methods." });

            var response = await _paymentClient.UpsertPaymentMethodAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Permanently drops a payment connector record.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            if (!_menuPerm.Has(User, SettingsMenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete payment methods." });

            var response = await _paymentClient.DeletePaymentMethodAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Enables/Disables frontend presence of a gateway.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> TogglePaymentStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, SettingsMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change payment method status." });

            var response = await _paymentClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        // Region: Legacy / Empty Layout Hooks
        public IActionResult BackupHistory() => View();
        public IActionResult PrintHeaderFooter() => View();
        public IActionResult SystemFields() => View();
        public IActionResult StudentProfile() => View();
        public IActionResult Modules() => View();
        public IActionResult FileType() => View();
        public IActionResult CustomFields() => View();
        public IActionResult Captcha() => View();
        public IActionResult Logo() => View();
        public IActionResult LoginPageBG() => View();
        public IActionResult StudentGuardian() => View();
        public IActionResult IDAutoGen() => View();
        public IActionResult AttendanceType() => View();
        public IActionResult Maintenance() => View();
        public IActionResult Miscellaneous() => View();
        /// <summary>
        /// Unified Company Master module setup.
        /// </summary>
        public async Task<IActionResult> Companies()
        {
            ViewData["Title"] = "Company Master";

            var companiesResponse = await _companyClient.GetAllAsync();
            var sessionsResponse = await _sessionClient.GetAllAsync();
            var currenciesResponse = await _currencyClient.GetAllAsync();

            var rawCompanies = companiesResponse.Success ? companiesResponse.Data : new List<MstCompanyViewModel>();
            var hierarchicalCompanies = new List<MstCompanyViewModel>();
            BuildCompanyHierarchy(rawCompanies, hierarchicalCompanies, null, 0);

            var model = new MstCompanyPageViewModel
            {
                Companies = hierarchicalCompanies,
                ParentCompanies = rawCompanies,
                Sessions = sessionsResponse.Success ? sessionsResponse.Data : new List<MstSessionViewModel>(),
                Currencies = currenciesResponse.Success ? currenciesResponse.Data : new List<MstCurrencyViewModel>()
            };

            return View(model);
        }

        private void BuildCompanyHierarchy(List<MstCompanyViewModel> source, List<MstCompanyViewModel> target, int? parentId, int level)
        {
            var children = source.Where(c => c.ParentCompanyId == parentId).OrderBy(c => c.SchoolName).ToList();
            foreach (var child in children)
            {
                child.Level = level;
                child.HasChildren = source.Any(c => c.ParentCompanyId == child.CompanyId);
                target.Add(child);
                BuildCompanyHierarchy(source, target, child.CompanyId, level + 1);
            }

            // Also add any orphaned nodes (where parent is not in source) at level 0 if this is the top-level call
            if (parentId == null)
            {
                var orphans = source.Where(c => c.ParentCompanyId != null && !source.Any(p => p.CompanyId == c.ParentCompanyId) && !target.Any(t => t.CompanyId == c.CompanyId)).OrderBy(c => c.SchoolName).ToList();
                foreach (var orphan in orphans)
                {
                    orphan.Level = 0;
                    orphan.HasChildren = source.Any(c => c.ParentCompanyId == orphan.CompanyId);
                    target.Add(orphan);
                    BuildCompanyHierarchy(source, target, orphan.CompanyId, 1);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCompany([FromBody] MstCompanyUpsertRequest request)
        {
            var isCreate = request.CompanyId <= 0;
            if (isCreate && !_menuPerm.Has(User, SettingsMenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add companies." });
            if (!isCreate && !_menuPerm.Has(User, SettingsMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit companies." });

            var response = await _companyClient.UpsertAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            if (!_menuPerm.Has(User, SettingsMenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete companies." });

            var response = await _companyClient.DeleteAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetCompany(int id)
        {
            if (!_menuPerm.Has(User, SettingsMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit companies.", data = (object?)null });

            var response = await _companyClient.GetByIDAsync(id);
            return Json(new { success = response.Success, data = response.Data, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleCompanyStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, SettingsMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change company status." });

            var response = await _companyClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Unified users index module bound under settings layout.
        /// Generates the list of Users and populates dropdown definitions via backend.
        /// </summary>
        public async Task<IActionResult> Users()
        {
            ViewData["Title"] = "Users";

            var usersResponse = await _userClient.GetAllUsersAsync();
            var rolesResponse = await _userClient.GetRolesDropdownAsync();
            var typesResponse = await _userClient.GetUserTypesDropdownAsync();

            var model = new UsersPageViewModel
            {
                Users = usersResponse.Success ? usersResponse.Data : new List<UserViewModel>(),
                Roles = rolesResponse.Success ? rolesResponse.Data : new List<RoleViewModel>(),
                UserTypes = typesResponse.Success ? typesResponse.Data : new List<MstUserTypeViewModel>()
            };

            return View(model);
        }

        /// <summary>
        /// Upserts an administrative user form.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveUser([FromBody] UserUpsertRequest request)
        {
            var isCreate = request.UserID <= 0;
            if (isCreate && !_menuPerm.Has(User, SettingsMenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add users." });
            if (!isCreate && !_menuPerm.Has(User, SettingsMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit users." });

            var response = await _userClient.SaveUserAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Toggles lock/active statuses.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int userId, bool isActive)
        {
            if (!_menuPerm.Has(User, SettingsMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change user status." });

            var response = await _userClient.ToggleStatusAsync(userId, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!_menuPerm.Has(User, SettingsMenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete users." });

            var response = await _userClient.DeleteUserAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }
    }
}
