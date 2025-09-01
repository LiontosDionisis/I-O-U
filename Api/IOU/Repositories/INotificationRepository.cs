using Api.IOU.Models;

namespace Api.IOU.Repositories;

public interface INotificationRepository
{
    Task<Notification> CreateAsync(Notification notification);
    Task<bool> DeleteAsync(int id);
    Task<Notification> GetbyId(int id);
    Task<IEnumerable<Notification>> GetAllAsync(int userId);

}