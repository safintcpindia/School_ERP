using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Services.Clients;
using SchoolERP.Net.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    public class TransportController : Controller
    {
        private readonly IPickupPointClientService _pickupPointClient;
        private readonly IRouteClientService _routeClient;
        private readonly IVehicleClientService _vehicleClient;
        private readonly IVehicleAssignClientService _vehicleAssignClient;
        private readonly IRoutePickupPointClientService _rppClient;

        public TransportController(
            IPickupPointClientService pickupPointClient, 
            IRouteClientService routeClient,
            IVehicleClientService vehicleClient,
            IVehicleAssignClientService vehicleAssignClient,
            IRoutePickupPointClientService rppClient)
        {
            _pickupPointClient = pickupPointClient;
            _routeClient = routeClient;
            _vehicleClient = vehicleClient;
            _vehicleAssignClient = vehicleAssignClient;
            _rppClient = rppClient;
        }

        public async Task<IActionResult> PickupPoint()
        {
            var res = await _pickupPointClient.GetAllPickupPointsAsync();
            var model = new PickupPointPageViewModel
            {
                Items = res.Success ? res.Data : new List<PickupPointViewModel>()
            };
            if (!res.Success) ViewBag.ErrorMessage = res.Message;
            return View(model);
        }

        public async Task<IActionResult> Routes()
        {
            var res = await _routeClient.GetAllRoutesAsync();
            var model = new RoutePageViewModel
            {
                Items = res.Success ? res.Data : new List<RouteViewModel>()
            };
            if (!res.Success) ViewBag.ErrorMessage = res.Message;
            return View(model);
        }

        public async Task<IActionResult> Vehicles()
        {
            var res = await _vehicleClient.GetAllVehiclesAsync();
            var model = new VehiclePageViewModel
            {
                Items = res.Success ? res.Data : new List<VehicleViewModel>()
            };
            if (!res.Success) ViewBag.ErrorMessage = res.Message;
            return View(model);
        }

        public async Task<IActionResult> VehicleAssign()
        {
            var res = await _vehicleAssignClient.GetAllAssignmentsAsync();
            var routesRes = await _routeClient.GetAllRoutesAsync();
            var vehiclesRes = await _vehicleClient.GetAllVehiclesAsync();

            var model = new VehicleAssignPageViewModel
            {
                Items = res.Success ? res.Data : new List<VehicleAssignViewModel>(),
                Routes = routesRes.Success ? routesRes.Data : new List<RouteViewModel>(),
                Vehicles = vehiclesRes.Success ? vehiclesRes.Data : new List<VehicleViewModel>()
            };
            
            if (!res.Success) ViewBag.ErrorMessage = res.Message;
            return View(model);
        }

        public async Task<IActionResult> RoutePickupPoints()
        {
            var res = await _rppClient.GetAllRoutePickupPointsAsync();
            var routesRes = await _routeClient.GetAllRoutesAsync();
            var pointsRes = await _pickupPointClient.GetAllPickupPointsAsync();

            var model = new RoutePickupPointPageViewModel
            {
                Items = res.Success ? res.Data : new List<RoutePickupPointViewModel>(),
                Routes = routesRes.Success ? routesRes.Data : new List<RouteViewModel>(),
                PickupPoints = pointsRes.Success ? pointsRes.Data : new List<PickupPointViewModel>()
            };
            
            if (!res.Success) ViewBag.ErrorMessage = res.Message;
            return View(model);
        }
    }
}
