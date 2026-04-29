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
    public class RoutePickupPointsApiController : ControllerBase
    {
        private readonly IRoutePickupPointService _rppService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;

        public RoutePickupPointsApiController(IRoutePickupPointService rppService, ICompanyService companySvc, ISessionService sessionSvc)
        {
            _rppService = rppService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
        }

        private int GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId"), out var id) ? id : 0;
        private int GetCompanyId() => _companySvc.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionSvc.GetUserCurrentSession(GetUserId()) ?? 0;

        [HttpGet("GetAllRoutePickupPoints")]
        public IActionResult GetAllRoutePickupPoints()
        {
            try
            {
                var data = _rppService.GetAllRoutePickupPoints(GetCompanyId(), GetSessionId());
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetRoutePickupPointByID/{id}")]
        public IActionResult GetRoutePickupPointByID(int id)
        {
            try
            {
                var data = _rppService.GetRoutePickupPointByID(id);
                if (data == null) return Ok(new { success = false, message = "Record not found" });
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("UpsertRoutePickupPoint")]
        public IActionResult UpsertRoutePickupPoint([FromBody] RoutePickupPointUpsertRequest req)
        {
            var res = _rppService.UpsertRoutePickupPoint(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteRoutePickupPoint/{id}")]
        public IActionResult DeleteRoutePickupPoint(int id)
        {
            var res = _rppService.DeleteRoutePickupPoint(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("ToggleRoutePickupPointStatus")]
        public IActionResult ToggleRoutePickupPointStatus(int id, bool isActive)
        {
            var res = _rppService.ToggleRoutePickupPointStatus(id, isActive, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }
    }
}
