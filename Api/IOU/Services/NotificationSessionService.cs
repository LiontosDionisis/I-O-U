using Api.IOU.Data;
using Api.IOU.DTOs;
using Api.IOU.Exceptions;
using Api.IOU.Models;

namespace Api.IOU.Services;

public class NotificationSessionService : INotificationSessionService
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationSessionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SessionNotificationDTO> CreateSessionNotificationAsync(SessionNotificationDTO notification)
    {
        var existingNotification = await _unitOfWork.SessionNotifications.GetById(notification.Id);
        if (existingNotification != null) throw new NotificationAlreadyExistsException("Notification exists with the same ID");

        var notificationToCreate = new NotificationSession
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Message = notification.Message,
            SessionId = notification.SessionId
        };

        await _unitOfWork.SessionNotifications.CreateAsync(notificationToCreate);
        await _unitOfWork.SaveChangesAsync();

        return ToSessionNotificationDTO(notificationToCreate);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var notification = await _unitOfWork.SessionNotifications.GetById(id);
        if (notification == null) throw new NotificationDoesNotExistException("Notification does not exist.");

        await _unitOfWork.SessionNotifications.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SessionNotificationDTO>> GetAllAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetById(userId);
        if (user == null) throw new UserNotFoundException("User does not exist");

        var notifications = await _unitOfWork.SessionNotifications.GetAllAsync(userId);
        var notificationsDto = notifications.Select(n => ToSessionNotificationDTO(n));

        return notificationsDto;
    }

    public async Task<SessionNotificationDTO> GetByIdAsync(int id)
    {
        var notification = await _unitOfWork.SessionNotifications.GetById(id);
        if (notification == null) throw new NotificationDoesNotExistException("Notification does not exist");

        return ToSessionNotificationDTO(notification);
    }

    private static SessionNotificationDTO ToSessionNotificationDTO(NotificationSession notification)
    {
        return new SessionNotificationDTO
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Message = notification.Message,
            SessionId = notification.SessionId
        };
    }
}