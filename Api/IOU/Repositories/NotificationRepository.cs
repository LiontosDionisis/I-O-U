using Api.IOU.Data;
using Api.IOU.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.IOU.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var notificationToDelete = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
        if (notificationToDelete == null) return false;
        _context.Notifications.Remove(notificationToDelete);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Notification>> GetAllAsync(int userId)
    {
        return await _context.Notifications.Where(u => u.UserId == userId).ToListAsync();
    }

    public async Task<Notification?> GetbyId(int id)
    {
        return await _context.Notifications.FirstOrDefaultAsync(i => i.Id == id);
    }
}