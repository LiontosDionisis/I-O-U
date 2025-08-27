using Api.IOU.DTOs;
using Api.IOU.Models;

namespace Api.IOU.Services;

public interface IFriendshipService
{
    Task<Friendship> SendFriendRequestAsync(int userId, int friendId);
    Task<Friendship> AcceptFriendRequestAsync(int requestId, int userId);
    Task<bool> DenyFriendRequestAsync(int requestId, int userId);
    Task<IEnumerable<FriendshipDTO>> GetPendingRequestsAsync(int userId);
    Task<IEnumerable<FriendshipDTO>> GetFriendsAsync(int userId);
    
}