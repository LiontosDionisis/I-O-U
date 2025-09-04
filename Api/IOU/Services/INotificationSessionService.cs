using Api.IOU.DTOs;

namespace Api.IOU.Services;

public interface INotificationSessionService
{
    Task<IEnumerable<SessionNotificationDTO>> GetAllAsync(int userId);
    Task<SessionNotificationDTO> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<SessionNotificationDTO> CreateSessionNotificationAsync(SessionNotificationDTO notification);
}