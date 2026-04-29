using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    public class HostelController : Controller
    {
        private readonly IHostelClientService _client;
        public HostelController(IHostelClientService client) => _client = client;


        public async Task<IActionResult> RoomType()
        {
            var res = await _client.GetAllRoomTypesAsync();
            var model = new RoomTypePageViewModel
            {
                Items = res.Success ? res.Data : new List<RoomTypeViewModel>()
            };
            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            var resHostel = await _client.GetAllHostelsAsync();
            var resRoomType = await _client.GetAllRoomTypesAsync();
            
            var model = new HostelPageViewModel
            {
                Items = resHostel.Success ? resHostel.Data : new List<HostelViewModel>(),
                RoomTypes = resRoomType.Success ? resRoomType.Data : new List<RoomTypeViewModel>()
            };
            return View(model);
        }

        public async Task<IActionResult> HostelRoom()
        {
            var resRoom = await _client.GetAllHostelRoomsAsync();
            var resHostel = await _client.GetAllHostelsAsync();
            var resRoomType = await _client.GetAllRoomTypesAsync();

            var model = new HostelRoomPageViewModel
            {
                Items = resRoom.Success ? resRoom.Data : new List<HostelRoomViewModel>(),
                Hostels = resHostel.Success ? resHostel.Data : new List<HostelViewModel>(),
                RoomTypes = resRoomType.Success ? resRoomType.Data : new List<RoomTypeViewModel>()
            };
            return View(model);
        }
    }
}
