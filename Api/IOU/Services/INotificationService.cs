using Api.IOU.DTOs;

namespace Api.IOU.Services;

public interface INotificationService
{
    Task<IEnumerable<NotificationDTO>> GetAllAsync(int userId);
    Task<NotificationDTO> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<NotificationDTO> CreateAsync(NotificationDTO dto);
}