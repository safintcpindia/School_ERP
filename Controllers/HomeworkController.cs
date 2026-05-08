using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    public class HomeworkController : Controller
    {
        private readonly IHomeworkClientService _homeworkClient;
        private readonly IClassClientService _classClient;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Homework/Add";

        public HomeworkController(IHomeworkClientService homeworkClient, IClassClientService classClient, IUserMenuPermissionService menuPerm)
        {
            _homeworkClient = homeworkClient;
            _classClient = classClient;
            _menuPerm = menuPerm;
        }

        public async Task<IActionResult> Add()
        {
            var homeworksResponse = await _homeworkClient.GetAllAsync();
            var classesResponse = await _classClient.GetAllAsync();

            var model = new HomeworkPageViewModel
            {
                Homeworks = homeworksResponse.Success ? homeworksResponse.Data : new List<HomeworkViewModel>(),
                Classes = classesResponse.Success ? classesResponse.Data : new List<MstClassViewModel>()
            };

            return View(model);
        }
    }
}
