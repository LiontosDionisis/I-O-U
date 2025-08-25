using Api.IOU.Data;
using api.IOU.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.IOU.Repositories;

public class SessionUserRepository : ISessionUserRepository
{
    private readonly AppDbContext _context;

    public SessionUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SessionUser> AddUserToSessionAsync(SessionUser sessionUser)
    {
        _context.Sessionusers.Add(sessionUser);
        await _context.SaveChangesAsync();
        return sessionUser;
    }

    public async Task<IEnumerable<SessionUser>> GetUsersBySessionIdAsync(int sessionId)
    {
        return await _context.Sessionusers.Include(su => su.User).Where(su => su.SessionId == sessionId).ToListAsync();
    }

    public async Task<bool> RemoveUserAsync(int userId, int sessionId)
    {
        var su = await _context.Sessionusers.FirstOrDefaultAsync(su => su.SessionId == sessionId && su.UserId == userId);
        if (su == null) return false;

        _context.Sessionusers.Remove(su);
        await _context.SaveChangesAsync();
        return true;
    }
}