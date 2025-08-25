using api.IOU.Models;

namespace Api.IOU.Repositories;

public interface ISessionUserRepository
{
    Task<SessionUser> AddUserToSessionAsync(SessionUser sessionUser);
    Task<IEnumerable<SessionUser>> GetUsersBySessionIdAsync(int sessionId);
    Task<bool> RemoveUserAsync(int userId, int sessionId);
}