using Api.IOU.DTOs;

namespace Api.IOU.Services;

public interface ISessionService
{
    Task<SessionDTO> CreateSessionAsync(int creatorId, string name);
    Task AddUserToSessionAsync(int sessionId, int ownerId, int userId);
    Task<IEnumerable<SessionDTO>> GetSessionsForUserAsync(int userId);
    Task<bool> RemoveUserFromSessionAsync(int sessionId, int userId, int friendId);
    Task<IEnumerable<UserDTO>> GetParticipantsAsync(int sessionId);
    Task<bool> DeleteSessionAsync(int sessionId, int userId);
}