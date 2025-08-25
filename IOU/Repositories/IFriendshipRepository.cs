using Api.IOU.Models;

namespace Api.IOU.Repositories;

public interface IFriendshipRepository
{
    Task<Friendship> CreateAsync(Friendship friendship);
    Task<Friendship?> GetByIdAsync(int id);
    Task<IEnumerable<Friendship>> GetFriendshipForUserAsync(int userId);
    Task<Friendship?> GetFriendshipBetweenUsersAsync(int userId, int friendId);
    Task<Friendship> UpdateAsync(Friendship friendship);
    Task<bool> DeleteAsync(int id);
}