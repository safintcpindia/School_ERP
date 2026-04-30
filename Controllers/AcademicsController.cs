using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This controller manages the school's academic structure, including Classes, Subjects, and how they are grouped together.
    /// </summary>
    public class AcademicsController : Controller
    {
        private readonly IClassClientService _classClient;
        private readonly ISectionClientService _sectionClient;
        private readonly ISubjectClientService _subjectClient;
        private readonly ISubjectGroupClientService _subjectGroupClient;
        private readonly IUserMenuPermissionService _menuPerm;
        
        private const string ClassMenuPath = "/Academics/Class";
        private const string SubjectMenuPath = "/Academics/Subject";
        private const string SubjectGroupMenuPath = "/Academics/SubjectGroup";

        public AcademicsController(
            IClassClientService classClient, 
            ISectionClientService sectionClient, 
            ISubjectClientService subjectClient, 
            ISubjectGroupClientService subjectGroupClient,
            IUserMenuPermissionService menuPerm)
        {
            _classClient = classClient;
            _sectionClient = sectionClient;
            _subjectClient = subjectClient;
            _subjectGroupClient = subjectGroupClient;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Shows the main 'Class' management page where you can see all classes and their assigned sections.
        /// </summary>
        public async Task<IActionResult> Class()
        {
            // Step 1: Ask the system for a list of all classes and all sections available.
            var classesResponse = await _classClient.GetAllAsync();
            var sectionsResponse = await _sectionClient.GetAllAsync();
            
            // Step 2: Prepare this information to be shown on the screen.
            var model = new MstClassPageViewModel
            {
                Classes = classesResponse.Success ? classesResponse.Data : new List<MstClassViewModel>(),
                AvailableSections = sectionsResponse.Success ? sectionsResponse.Data : new List<MstSectionViewModel>()
            };
            
            // Step 3: Open the 'Class' management page.
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetClass(int id)
        {
            if (!_menuPerm.Has(User, ClassMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit classes." });

            var response = await _classClient.GetByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

        /// <summary>
        /// Saves a new class or updates the details of an existing one.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveClass([FromBody] MstClassUpsertRequest request)
        {
            // Step 1: Check if the user is allowed to add or edit classes based on whether the class already exists.
            var isCreate = request.ClassID <= 0;
            if (isCreate && !_menuPerm.Has(User, ClassMenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add classes." });
            if (!isCreate && !_menuPerm.Has(User, ClassMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit classes." });
            
            // Step 2: Send the new class details to the backend system to be saved.
            var response = await _classClient.UpsertAsync(request);
            
            // Step 3: Inform the user if the class was saved successfully.
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleClassStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, ClassMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change class status." });

            var response = await _classClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClass(int id)
        {
            if (!_menuPerm.Has(User, ClassMenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete classes." });

            var response = await _classClient.DeleteAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }

        public async Task<IActionResult> Subject()
        {
            var response = await _subjectClient.GetAllAsync();
            var model = new MstSubjectPageViewModel
            {
                Subjects = response.Success ? response.Data : new List<MstSubjectViewModel>()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubject(int id)
        {
            if (!_menuPerm.Has(User, SubjectMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit subjects." });

            var response = await _subjectClient.GetByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSubject([FromBody] MstSubjectUpsertRequest request)
        {
            var isCreate = request.SubjectID <= 0;
            if (isCreate && !_menuPerm.Has(User, SubjectMenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add subjects." });
            if (!isCreate && !_menuPerm.Has(User, SubjectMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit subjects." });

            var response = await _subjectClient.UpsertAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleSubjectStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, SubjectMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change subject status." });

            var response = await _subjectClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            if (!_menuPerm.Has(User, SubjectMenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete subjects." });

            var response = await _subjectClient.DeleteAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetSectionsByClass(int id)
        {
            var response = await _sectionClient.GetByClassAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

        /// <summary>
        /// Shows the 'Subject Group' page where you can link subjects to specific classes and sections.
        /// </summary>
        public async Task<IActionResult> SubjectGroup()
        {
            // Step 1: Gather all the necessary lists (Groups, Classes, Sections, and Subjects) from the system.
            var groupsResponse = await _subjectGroupClient.GetAllAsync();
            var classesResponse = await _classClient.GetAllAsync();
            var sectionsResponse = await _sectionClient.GetAllAsync();
            var subjectsResponse = await _subjectClient.GetAllAsync();
 
            // Step 2: Organize all this data to be used on the subject grouping screen.
            var model = new MstSubjectGroupPageViewModel
            {
                SubjectGroups = groupsResponse.Success ? groupsResponse.Data : new List<MstSubjectGroupViewModel>(),
                Classes = classesResponse.Success ? classesResponse.Data : new List<MstClassViewModel>(),
                Sections = sectionsResponse.Success ? sectionsResponse.Data : new List<MstSectionViewModel>(),
                Subjects = subjectsResponse.Success ? subjectsResponse.Data : new List<MstSubjectViewModel>()
            };
 
            // Step 3: Open the 'Subject Group' management page.
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjectGroup(int id)
        {
            if (!_menuPerm.Has(User, SubjectGroupMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit subject groups." });

            var response = await _subjectGroupClient.GetByIDAsync(id);
            if (!response.Success) return Json(new { success = false, message = response.Message });
            return Json(new { success = true, data = response.Data });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSubjectGroup([FromBody] MstSubjectGroupUpsertRequest request)
        {
            var isCreate = request.SubjectGroupID <= 0;
            if (isCreate && !_menuPerm.Has(User, SubjectGroupMenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add subject groups." });
            if (!isCreate && !_menuPerm.Has(User, SubjectGroupMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit subject groups." });

            var response = await _subjectGroupClient.UpsertAsync(request);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleSubjectGroupStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, SubjectGroupMenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change subject group status." });

            var response = await _subjectGroupClient.ToggleStatusAsync(id, isActive);
            return Json(new { success = response.Success, message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubjectGroup(int id)
        {
            if (!_menuPerm.Has(User, SubjectGroupMenuPath, "Delete"))
                return Json(new { success = false, message = "You do not have permission to delete subject groups." });

            var response = await _subjectGroupClient.DeleteAsync(id);
            return Json(new { success = response.Success, message = response.Message });
        }

        public IActionResult ClassTimeTable()
        {
            return View();
        }
    }
}
