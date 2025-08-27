using Api.IOU.Data;
using Api.IOU.DTOs;
using Api.IOU.Exceptions;
using Api.IOU.Models;
using api.IOU.Models;

namespace Api.IOU.Services;

public class SessionService : ISessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SessionService> _logger;

    public SessionService(IUnitOfWork unitOfWork, ILogger<SessionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task AddUserToSessionAsync(int sessionId, int ownerId, int userId)
    {
        // Check if session exists.
        var session = await _unitOfWork.Sessions.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new SessionNotFoundException("Session does not exist.");
        }

        // Check if user exists.
        var userToAdd = await _unitOfWork.Users.GetById(userId);
        if (userToAdd == null)
        {
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

        await _unitOfWork.SessionUsers.AddUserToSessionAsync(sessionUser);
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

        return newSession;
    }

    public async Task<bool> DeleteSessionAsync(int sessionId)
    {
        var sessionToDelete = await _unitOfWork.Sessions.GetByIdAsync(sessionId);

        if (sessionToDelete == null)
        {
            throw new SessionNotFoundException("Session does not exist.");
        }

        await _unitOfWork.Sessions.DeleteAsync(sessionId);
        await _unitOfWork.SaveChangesAsync();

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
            Email = u.User.Email
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
                Username = p.User.Username
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
            await _unitOfWork.SessionUsers.RemoveUserAsync(sessionId, friendId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        else
        {
            throw new UnauthorizedAccessException("Only session owner can remove users");
        }
        
    }
}

