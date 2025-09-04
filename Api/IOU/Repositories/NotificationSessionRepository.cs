using Api.IOU.Data;
using Api.IOU.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.IOU.Repositories;

public class NotificationSessionRepository : INotificationSessionRepository
{
    private readonly AppDbContext _context;

    public NotificationSessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationSession> CreateAsync(NotificationSession notification)
    {
        _context.SessionNotifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var notification = await _context.SessionNotifications.FirstOrDefaultAsync(n => n.Id == id);
        if (notification == null) return false;
        _context.SessionNotifications.Remove(notification);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<NotificationSession>> GetAllAsync(int userId)
    {
        return await _context.SessionNotifications.Where(u => u.Id == userId).ToListAsync();
    }

    public async Task<NotificationSession?> GetById(int id)
    {
        return await _context.SessionNotifications.FirstOrDefaultAsync(n => n.Id == id);
    }
}