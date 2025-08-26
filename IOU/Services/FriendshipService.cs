using Api.IOU.Data;
using Api.IOU.Exceptions;
using Api.IOU.Models;
using Api.IOU.Repositories;

namespace Api.IOU.Services;

public class FriendshipService : IFriendshipService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FriendshipService> _logger;

    public FriendshipService(IUnitOfWork unitOfWork, ILogger<FriendshipService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Friendship> AcceptFriendRequestAsync(int requestId, int userId)
    {
        var friendship = await _unitOfWork.Friendships.GetByIdAsync(requestId);
        if (friendship == null)
        {
            throw new FriendRequestNotFoundException("Friend request not found.");
        }

        if (friendship.FriendId != userId)
        {
            throw new InvalidFriendRequest("You can only accept requests sent to you.");
        }

        friendship.Status = FriendshipStatus.Accepted;
        return await _unitOfWork.Friendships.UpdateAsync(friendship);
    }

    public async Task<bool> DenyFriendRequestAsync(int reqeustId, int userId)
    {
        var friendship = await _unitOfWork.Friendships.GetByIdAsync(reqeustId);
        if (friendship == null)
        {
            throw new FriendRequestNotFoundException("Friend request not found.");
        }

        if (friendship.FriendId != userId)
        {
            throw new InvalidFriendRequest("You can only deny requests sent to you.");
        }

        return await _unitOfWork.Friendships.DeleteAsync(reqeustId);
    }

    public async Task<IEnumerable<Friendship>> GetFriendsAsync(int userId)
    {
        var allFriendships = await _unitOfWork.Friendships.GetFriendshipForUserAsync(userId);
        return allFriendships.Where(f => f.Status == FriendshipStatus.Accepted);
    }

    public async Task<IEnumerable<Friendship>> GetPendingRequestsAsync(int userId)
    {
        var allFriendships = await _unitOfWork.Friendships.GetFriendshipForUserAsync(userId);

        return allFriendships.Where(f => f.Status == FriendshipStatus.Pending && f.FriendId == userId); // Filters requests where user is recipent.
    }

    public async Task<Friendship> SendFriendRequestAsync(int userId, int friendId)
    {
        if (userId == friendId)
        {
            throw new InvalidFriendRequest("Invalid Friend Request.");
        }

        var existingFriendship = await _unitOfWork.Friendships.GetFriendshipBetweenUsersAsync(userId, friendId);

        if (existingFriendship != null)
        {
            throw new InvalidFriendRequest("Friendship already exists");
        }

        var friendship = new Friendship
        {
            UserId = userId,
            FriendId = friendId,
            Status = FriendshipStatus.Pending
        };

        return await _unitOfWork.Friendships.CreateAsync(friendship);
    }
}