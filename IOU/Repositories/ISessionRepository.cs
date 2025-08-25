using Api.IOU.Models;

namespace Api.IOU.Repositories;

public interface ISessionRepository
{
    Task<Session> CreateAsync(Session session);
    Task<Session?> GetByIdAsync(int id);
    Task<IEnumerable<Session>> GetAllAsync();
    Task<IEnumerable<Session>> GetSessionsForUserAsync(int userId);
    Task<Session> UpdateAsync(Session session);
    Task<bool> DeleteAsync(int id);
}