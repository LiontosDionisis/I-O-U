using Api.IOU.Data;
using Api.IOU.DTOs;
using Api.IOU.Exceptions;
using Api.IOU.Models;
using Api.IOU.Repositories;

namespace Api.IOU.Services;

//TODO: Fix Accepting friend request sending all users information as JSON (remove email and password: Use a FriendshipDTO)
public class FriendshipService : IFriendshipService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notService;
    private readonly ILogger<FriendshipService> _logger;

    public FriendshipService(IUnitOfWork unitOfWork, ILogger<FriendshipService> logger, INotificationService notService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _notService = notService;
    }

    public async Task<Friendship> AcceptFriendRequestAsync(int requestId, int userId)
    {
        var friendship = await _unitOfWork.Friendships.GetByIdAsync(requestId);
        if (friendship == null)
        {
            _logger.LogWarning("Friend request with ID {Id} was not found", requestId);
            throw new FriendRequestNotFoundException("Friend request not found.");
        }

        if (friendship.FriendId != userId)
        {
            _logger.LogWarning("Cannot accept foreign friend requests.");
            throw new InvalidFriendRequest("You can only accept requests sent to you.");
        }

        _logger.LogInformation("Friend request with ID {Id} accepted", requestId);
        friendship.Status = FriendshipStatus.Accepted;

        var user = await _unitOfWork.Users.GetById(friendship.FriendId);
        var notification = new NotificationDTO
        {
            UserId = friendship.UserId,                    
            Message = $"{user!.Username} has accepted your friend request!"
        };
        await _notService.CreateAsync(notification);

        return await _unitOfWork.Friendships.UpdateAsync(friendship);
    }

    public async Task<bool> DenyFriendRequestAsync(int requestId, int userId)
    {
        var friendship = await _unitOfWork.Friendships.GetByIdAsync(requestId);
        if (friendship == null)
        {
            _logger.LogWarning("Friend request with ID {Id} was not found", requestId);
            throw new FriendRequestNotFoundException("Friend request not found.");
        }

        if (friendship.FriendId != userId)
        {
            _logger.LogWarning("Cannot deny foreign friend requests.");
            throw new InvalidFriendRequest("You can only deny requests sent to you.");
        }

        _logger.LogInformation("Friend request with ID {Id} denied", requestId);
        return await _unitOfWork.Friendships.DeleteAsync(requestId);
    }

    public async Task<IEnumerable<FriendshipDTO>> GetFriendsAsync(int userId)
    {
        var allFriendships = await _unitOfWork.Friendships.GetFriendshipForUserAsync(userId);

        _logger.LogInformation("Friendlist for user with ID {Id} was found and returned", userId);

        return allFriendships.Where(f => f.Status == FriendshipStatus.Accepted).Select(f => new FriendshipDTO
        {
            Id = f.Id,
            FriendId = f.UserId == userId ? f.FriendId : f.UserId,
            FriendUsername = f.UserId == userId ? f.Friend.Username : f.User.Username,
            Status = f.Status
        });
    }

    public async Task<IEnumerable<FriendshipDTO>> GetPendingRequestsAsync(int userId)
    {
        var allFriendships = await _unitOfWork.Friendships.GetFriendshipForUserAsync(userId);

        _logger.LogInformation("Friend requestes for user with ID {Id} were found and returned", userId);

        return allFriendships
        .Where(f => f.Status == FriendshipStatus.Pending && f.FriendId == userId)
        .Select(f => new FriendshipDTO
        {
            Id = f.Id,
            FriendId = f.UserId,
            FriendUsername = f.User.Username, 
            Status = f.Status
        });
    }

    public async Task<Friendship> SendFriendRequestAsync(int userId, int friendId)
    {
        if (userId == friendId)
        {
            _logger.LogWarning("Invalid friend request for user with ID {Id}", userId);
            throw new InvalidFriendRequest("Invalid Friend Request.");
        }

        var existingFriendship = await _unitOfWork.Friendships.GetFriendshipBetweenUsersAsync(userId, friendId);

        if (existingFriendship != null)
        {
            _logger.LogWarning("Friendship already exists for user with ID {Id} and user with ID {friendId}", userId, friendId);
            throw new InvalidFriendRequest("Friendship already exists");
        }

        var friendship = new Friendship
        {
            UserId = userId,
            FriendId = friendId,
            Status = FriendshipStatus.Pending
        };
        var user = await _unitOfWork.Users.GetById(userId);

        var notification = new NotificationDTO
        {
            UserId = friendId,
            Message = $"{user!.Username} wants to be your friend"
        };

        await _notService.CreateAsync(notification);
        return await _unitOfWork.Friendships.CreateAsync(friendship);
    }

    
}