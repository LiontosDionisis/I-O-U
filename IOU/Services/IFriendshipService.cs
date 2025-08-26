using Api.IOU.Models;

namespace Api.IOU.Services;

public interface IFriendshipService
{
    Task<Friendship> SendFriendRequestAsync(int userId, int friendId);
    Task<Friendship> AcceptFriendRequestAsync(int requestId, int userId);
    Task<bool> DenyFriendRequestAsync(int reqeustId, int userId);
    Task<IEnumerable<Friendship>> GetPendingRequestsAsync(int userId);
    Task<IEnumerable<Friendship>> GetFriendsAsync(int userId);
}