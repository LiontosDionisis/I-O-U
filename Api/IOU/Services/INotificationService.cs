using Api.IOU.DTOs;
using Api.IOU.Models;

namespace Api.IOU.Services;

public interface INotificationService
{
    Task<IEnumerable<FriendNotificationDTO>> GetAllAsync(int userId);
    Task<FriendNotificationDTO> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<FriendNotificationDTO> CreateFriendNotificationAsync(FriendNotificationDTO notification);
    
}