using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesApiController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;

        public RoutesApiController(IRouteService routeService, ICompanyService companySvc, ISessionService sessionSvc)
        {
            _routeService = routeService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
        }

        private int GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId"), out var id) ? id : 0;
        private int GetCompanyId() => _companySvc.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionSvc.GetUserCurrentSession(GetUserId()) ?? 0;

        [HttpGet("GetAllRoutes")]
        public IActionResult GetAllRoutes()
        {
            var data = _routeService.GetAllRoutes(GetCompanyId(), GetSessionId());
            return Ok(new { success = true, data });
        }

        [HttpGet("GetRouteByID/{id}")]
        public IActionResult GetRouteByID(int id)
        {
            var data = _routeService.GetRouteByID(id);
            if (data == null) return Ok(new { success = false, message = "Record not found" });
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertRoute")]
        public IActionResult UpsertRoute([FromBody] RouteUpsertRequest req)
        {
            var res = _routeService.UpsertRoute(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteRoute/{id}")]
        public IActionResult DeleteRoute(int id)
        {
            var res = _routeService.DeleteRoute(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("ToggleRouteStatus")]
        public IActionResult ToggleRouteStatus(int id, bool isActive)
        {
            var res = _routeService.ToggleRouteStatus(id, isActive, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }
    }
}
