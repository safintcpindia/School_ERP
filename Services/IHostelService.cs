using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IHostelService
    {
        // Room Type
        List<RoomTypeViewModel> GetAllRoomTypes(int companyId, int sessionId, bool includeDeleted = false);
        RoomTypeViewModel? GetRoomTypeByID(int id);
        (bool Success, string Message) UpsertRoomType(RoomTypeUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteRoomType(int id, int userId);
        (bool Success, string Message) ToggleRoomTypeStatus(int id, bool isActive, int userId);

        // Hostel
        List<HostelViewModel> GetAllHostels(int companyId, int sessionId, bool includeDeleted = false);
        HostelViewModel? GetHostelByID(int id);
        (bool Success, string Message) UpsertHostel(HostelUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteHostel(int id, int userId);
        (bool Success, string Message) ToggleHostelStatus(int id, bool isActive, int userId);

        // Hostel Room
        List<HostelRoomViewModel> GetAllHostelRooms(int companyId, int sessionId, bool includeDeleted = false);
        HostelRoomViewModel? GetHostelRoomByID(int id);
        (bool Success, string Message) UpsertHostelRoom(HostelRoomUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteHostelRoom(int id, int userId);
        (bool Success, string Message) ToggleHostelRoomStatus(int id, bool isActive, int userId);
    }
}
