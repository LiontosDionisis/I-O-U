using Api.IOU.Data;
using Api.IOU.DTOs;
using Api.IOU.Exceptions;
using Api.IOU.Models;

namespace Api.IOU.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FriendNotificationDTO> CreateFriendNotificationAsync(FriendNotificationDTO notification)
    {
        var existingNot = await _unitOfWork.Notifications.GetbyId(notification.Id);
        if (existingNot != null) throw new NotificationAlreadyExistsException("Notification already exists with the same id");
        var notificationToCreate = new Notification
        {
            Id = notification.Id,
            UserId = notification.UserId,
            FriendshipId = notification.FriendshipId,
            Message = notification.Message
        };

        await _unitOfWork.Notifications.CreateAsync(notificationToCreate);
        await _unitOfWork.SaveChangesAsync();
        return ToFriendNotificationDTO(notificationToCreate);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var notToDelete = await _unitOfWork.Notifications.GetbyId(id);
        if (notToDelete == null) throw new NotificationDoesNotExistException("No notification with this ID");

        await _unitOfWork.Notifications.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<FriendNotificationDTO>> GetAllAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetById(userId);
        if (user == null) throw new UserNotFoundException("User does not exist");

        var notifications = await _unitOfWork.Notifications.GetAllAsync(userId);
        var notificationsDto = notifications.Select(n => ToFriendNotificationDTO(n));

        return notificationsDto;
    }

    public async Task<FriendNotificationDTO> GetByIdAsync(int id)
    {
        var notification = await _unitOfWork.Notifications.GetbyId(id);
        return ToFriendNotificationDTO(notification);
    }

    private static FriendNotificationDTO ToFriendNotificationDTO(Notification notification)
    {
        return new FriendNotificationDTO
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Message = notification.Message,
            FriendshipId = notification.FriendshipId,
        };
    }
}