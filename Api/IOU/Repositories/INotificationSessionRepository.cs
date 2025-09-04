using Api.IOU.Models;

namespace Api.IOU.Repositories;

public interface INotificationSessionRepository
{
    Task<NotificationSession> CreateAsync(NotificationSession notification);
    Task<bool> DeleteAsync(int id);
    Task<NotificationSession> GetById(int id);
    Task<IEnumerable<NotificationSession>> GetAllAsync(int userId);
}