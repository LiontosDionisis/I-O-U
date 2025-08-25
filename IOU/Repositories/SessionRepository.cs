using Api.IOU.Data;
using Api.IOU.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.IOU.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _context;

    public SessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Session> CreateAsync(Session session)
    {
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null) return false;

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Session>> GetAllAsync()
    {
        return await _context.Sessions.Include(s => s.Participants).ThenInclude(su => su.User).Include(s => s.Expenses).ThenInclude(e => e.Splits).ToListAsync();
    }

    public async Task<Session?> GetByIdAsync(int id)
    {
        return await _context.Sessions.Include(s => s.Participants).ThenInclude(su => su.User).Include(s => s.Expenses).ThenInclude(e => e.Splits).FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Gets user's sessions.
    /// </summary>
    /// <param name="userId">User's ID</param>
    /// <returns>A list of user's sessions.</returns>
    public async Task<IEnumerable<Session>> GetSessionsForUserAsync(int userId)
    {
        return await _context.Sessions.Include(s => s.Participants).ThenInclude(su => su.User).Where(s => s.Participants.Any(p => p.UserId == userId)).ToListAsync();
    }

    public async Task<Session> UpdateAsync(Session session)
    {
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }
}