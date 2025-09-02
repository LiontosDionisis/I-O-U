using Api.IOU.Data;
using Api.IOU.DTOs;
using Api.IOU.Exceptions;
using Api.IOU.Models;

namespace Api.IOU.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationService> _logger;


    public NotificationService(IUnitOfWork unitOfWork, ILogger<NotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<NotificationDTO> CreateAsync(NotificationDTO dto)
    {
        var notification = await _unitOfWork.Notifications.GetbyId(dto.Id);
        if (notification != null) throw new NotificationAlreadyExistsException("Notification exists.");

        var newNotification = new Notification
        {
            Id = dto.Id,
            Message = dto.Message,
            UserId = dto.UserId
            
        };

        await _unitOfWork.Notifications.CreateAsync(newNotification);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Notification with ID {id} has been created!", dto.Id);

        return ToNotificationDto(newNotification);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var notificationToDelete = await _unitOfWork.Notifications.GetbyId(id);
        if (notificationToDelete == null) throw new NotificationDoesNotExistException("Notification does not exist.");

        var deleted = await _unitOfWork.Notifications.DeleteAsync(notificationToDelete.Id);
        if (deleted)
        {
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Notification deleted with ID {Id}", id);
        }
        else
        {
            _logger.LogWarning("Failed to delete notification");
        }
        return deleted;
    }

    public async Task<IEnumerable<NotificationDTO>> GetAllAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetById(userId);
        if (user == null) throw new UserNotFoundException("User does not exist.");

        var notifications = await _unitOfWork.Notifications.GetAllAsync(userId);
        if (notifications == null) throw new NoNotificationsException("You have no notifications.");
        
        return notifications.Select(u => ToNotificationDto(u));
    }

    public async Task<NotificationDTO> GetByIdAsync(int id)
    {
        var notification = await _unitOfWork.Notifications.GetbyId(id);
        if (notification == null) throw new NotificationDoesNotExistException("Notification does not exist");

        return ToNotificationDto(notification);
    }

    private static NotificationDTO ToNotificationDto(Notification notification)
    {
        return new NotificationDTO
        {
            Id = notification.Id,
            Message = notification.Message,
            UserId = notification.UserId
        };
    }

}