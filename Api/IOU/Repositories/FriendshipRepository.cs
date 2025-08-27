using Api.IOU.Data;
using Api.IOU.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.IOU.Repositories;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly AppDbContext _context;

    public FriendshipRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Friendship> CreateAsync(Friendship friendship)
    {
        _context.Friendships.Add(friendship);
        await _context.SaveChangesAsync();
        return friendship;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var friendship = await _context.Friendships.FindAsync(id);
        if (friendship == null) return false;

        _context.Remove(friendship);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Friendship?> GetByIdAsync(int id)
    {
        return await _context.Friendships.FindAsync(id);
    }

    /// <summary>
    /// Gets the friendship between two users, if it exists.
    /// </summary>
    /// <param name="userId">User's ID</param>
    /// <param name="friendId">Another user's ID</param>
    /// <returns>The friendship object between the two users or null if no friendship exists.</returns>
    public async Task<Friendship?> GetFriendshipBetweenUsersAsync(int userId, int friendId)
    {
        return await _context.Friendships.FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId));
    }


    /// <summary>
    /// Gets all user's friends.
    /// </summary>
    /// <param name="userId">User's ID</param>
    /// <returns>A list of user's friendships (friendlist)</returns>
    public async Task<IEnumerable<Friendship>> GetFriendshipForUserAsync(int userId)
    {
        return await _context.Friendships.Where(f => f.UserId == userId || f.FriendId == userId).Include(f => f.User).Include(f => f.Friend).ToListAsync();
    }

    public async Task<Friendship> UpdateAsync(Friendship friendship)
    {
        _context.Friendships.Update(friendship);
        await _context.SaveChangesAsync();
        return friendship;
    }
}