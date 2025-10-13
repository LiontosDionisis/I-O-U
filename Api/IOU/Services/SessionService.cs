using Api.IOU.Data;
using Api.IOU.DTOs;
using Api.IOU.Exceptions;
using Api.IOU.Models;
using api.IOU.Models;
using Microsoft.AspNetCore.SignalR;
using Api.IOU.Hubs;

namespace Api.IOU.Services;

public class SessionService : ISessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SessionService> _logger;
    private readonly IHubContext<SessionHub> _hubContext;

    public SessionService(IUnitOfWork unitOfWork, ILogger<SessionService> logger, IHubContext<SessionHub> hubContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task AddUserToSessionAsync(int sessionId, int ownerId, int userId)
    {
        // Check if session exists.
        var session = await _unitOfWork.Sessions.GetByIdAsync(sessionId);
        if (session == null)
        {
            _logger.LogWarning("Session does not exist with ID {SessionId}", sessionId);
            throw new SessionNotFoundException("Session does not exist.");
        }

        // Check if user exists.
        var userToAdd = await _unitOfWork.Users.GetById(userId);
        if (userToAdd == null)
        {
            _logger.LogWarning("User with ID {UserId} does not exist.", userId);
            throw new UserNotFoundException("User you're trying to add does not exist");
        }

        // Check if Users are friends.
        var friendship = await _unitOfWork.Friendships.GetFriendshipBetweenUsersAsync(ownerId, userId);
        if (friendship == null)
        {
            throw new CannotAddUserToSessionException("User is not your friend.");
        }

        var sessionUser = new SessionUser
        {
            SessionId = sessionId,
            UserId = userId
        };

        var sender = await _unitOfWork.Users.GetById(ownerId);
        var notification = new NotificationSession
        {
            Message = $"{sender!.Username} has added you to a Session '{session.Name}'",
            SessionId = session.Id,
            UserId = userId
        };
        //TODO: Fix notification notifying yourself when creating a session (if (ownerId != userId)
        await _unitOfWork.SessionUsers.AddUserToSessionAsync(sessionUser);
        await _unitOfWork.SessionNotifications.CreateAsync(notification);
        await _unitOfWork.SaveChangesAsync();
        
    }

    public async Task<SessionDTO> CreateSessionAsync(int creatorId, string name)
    {
        var session = new Session
        {
            Name = name,
            CreatedById = creatorId,
            CreatedAt = DateTime.UtcNow
        };
    

        await _unitOfWork.Sessions.CreateAsync(session);
        await _unitOfWork.SaveChangesAsync();

        await _unitOfWork.SessionUsers.AddUserToSessionAsync(new SessionUser
        {
            SessionId = session.Id,
            UserId = creatorId
        });

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Session {SessionName} created by user {UserId}", name, creatorId);

        
        var user = await _unitOfWork.Users.GetById(creatorId);

        var newSession = new SessionDTO
        {
            Id = session.Id,
            Name = session.Name,

            Participants = new List<ParticipantDTO>
            {
                new ParticipantDTO{
                    UserId = creatorId,
                    Username = user!.Username
                }
            },

            CreatedAt = session.CreatedAt,
            CreatedById = session.CreatedById
        };

        _logger.LogInformation("Session Created with ID: {SessionId}", newSession.Id);


        // SignalR Real time update (Will only send signal to the users in the session meaning the creator)
        var participansIds = await _unitOfWork.SessionUsers.GetUsersBySessionIdAsync(newSession.Id);

        await _hubContext.Clients.Users(participansIds.Select(id => id.ToString())!).SendAsync("SessionCreated", newSession); 

        return newSession;
    }

    public async Task<bool> DeleteSessionAsync(int sessionId, int userId)
    {
        var sessionToDelete = await _unitOfWork.Sessions.GetByIdAsync(sessionId);
        var owner = await _unitOfWork.Users.GetById(userId);
        var participants = sessionToDelete!.Participants.Select(p => p.UserId.ToString());

        if (sessionToDelete == null)
        {
            throw new SessionNotFoundException("Session does not exist.");
        }

        if (owner!.Id == sessionToDelete.CreatedById)
        {
            await _unitOfWork.Sessions.DeleteAsync(sessionId);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new UnauthorizedAccessException("You're not the session owner.");
        }

        await _hubContext.Clients.Users(participants).SendAsync("SessionDeleted", sessionId);

        _logger.LogInformation("Session with ID {SessionId} was deleted!", sessionId);
        return true;
    }

    public async Task<IEnumerable<UserDTO>> GetParticipantsAsync(int sessionId)
    {
        var existingSession = await _unitOfWork.Sessions.GetByIdAsync(sessionId);
        if (existingSession == null) throw new SessionNotFoundException("Session does not exist.");

        var users = await _unitOfWork.SessionUsers.GetUsersBySessionIdAsync(sessionId);
        return users.Select(u => new UserDTO
        {
            Id = u.UserId,
            Username = u.User.Username,
            Email = u.User.Email,
            Avatar = u.User.Avatar
        });
    }

    public async Task<IEnumerable<SessionDTO>> GetSessionsForUserAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetById(userId);
        if (user == null) throw new UserNotFoundException("User does not exist");

        var sessionUsers = await _unitOfWork.SessionUsers.GetSessionsForUserAsync(userId);

        var sessionsDto = sessionUsers.Select(su => su.Session).Distinct().Select(session => new SessionDTO
        {
            Id = session.Id,
            Name = session.Name,
            CreatedById = session.CreatedById,
            CreatedAt = session.CreatedAt,
            Participants = session.Participants.Select(p => new ParticipantDTO
            {
                UserId = p.UserId,
                Username = p.User.Username,
                Avatar = p.User.Avatar
            }).ToList()
        });

        return sessionsDto;
    }

    public async Task<bool> RemoveUserFromSessionAsync(int sessionId, int userId, int friendId)
    {
        var existingSession = await _unitOfWork.Sessions.GetByIdAsync(sessionId);
        if (existingSession == null) throw new SessionNotFoundException($"Session with ID {sessionId} does not exist.");

        var userToRemove = await _unitOfWork.Users.GetById(friendId);
        if (userToRemove == null) throw new UserNotFoundException("User does not exist");

        if (friendId == existingSession.CreatedById)
        {
            throw new InvalidOperationException("Cannot remove yourself from the session");
        }

        if (existingSession.CreatedById == userId)
        {
            await _unitOfWork.SessionUsers.RemoveUserAsync(friendId, sessionId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        else
        {
            throw new UnauthorizedAccessException("Only session owner can remove users");
        }
        
    }
}

