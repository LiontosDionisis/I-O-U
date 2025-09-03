using System.Security.Claims;
using Api.IOU.DTOs;
using Api.IOU.Exceptions;
using Api.IOU.Models;
using Api.IOU.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.IOU.Controllers;

[ApiController]
[Route("api/sessions")]
[Authorize]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly ILogger<SessionController> _logger;

    public SessionController(ISessionService sessionService, ILogger<SessionController> logger)
    {
        _sessionService = sessionService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionDTO dto)
    {
        var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            var session = await _sessionService.CreateSessionAsync(userId, dto.Name);
            return Ok(session);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create session");
            return StatusCode(500, new { message = "Failed to create session" });
        }
    }

    [HttpPost("{sessionId}/add/{friendId}")]
    public async Task<IActionResult> AddUserToSession(int sessionId, int friendId)
    {
        var ownerId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            await _sessionService.AddUserToSessionAsync(sessionId, ownerId, friendId);
            return Ok(new { message = "User added to session!" });
        }
        catch (SessionNotFoundException e)
        {
            _logger.LogError(e, "Session does not exist");
            return NotFound(new { message = "Session was not found or does not exist." });
        }
        catch (UserNotFoundException e)
        {
            _logger.LogError(e, "User with ID {UserId} was not found", friendId);
            return NotFound(new { message = "User was not found." });
        }
        catch (CannotAddUserToSessionException e)
        {
            _logger.LogError(e, "Cannot add user to session");
            return BadRequest(new { message = "Cannot add user to session" });
        }
    }

    [HttpDelete("{sessionId}/remove/{friendId}")]
    public async Task<IActionResult> RemoveUserFromSession(int sessionId, int friendId)
    {
        var ownerId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            await _sessionService.RemoveUserFromSessionAsync(sessionId, ownerId, friendId);
            return Ok(new { message = "User has been removed from session" });
        }
        catch (SessionNotFoundException e)
        {
            _logger.LogError(e, "Session with ID {SessionId} was not found.", sessionId);
            return NotFound("Session not found");
        }
        catch (UserNotFoundException e)
        {
            _logger.LogError(e, "User with ID {UserId} was not found.", friendId);
            return NotFound("User was not found");
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError(e, "Cannot remove yourself from session");
            return BadRequest("Cannot remove yourself from session");
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogError(e, "You're not the session owner");
            return BadRequest("You are not the session owner.");
        }
    }

    [HttpGet("{sessionId}")]
    public async Task<IActionResult> GetParticiapnts(int sessionId)
    {
        var ownerId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            var participants = await _sessionService.GetParticipantsAsync(sessionId);
            return Ok(participants);
        }
        catch (SessionNotFoundException e)
        {
            _logger.LogError(e, "Session was not found with ID {sessionId}", sessionId);
            return NotFound("Session was not found");
        }
    }


    [HttpGet("my-sessions")]
    public async Task<IActionResult> GetMySessions()
    {
        var ownerId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        try
        {
            var sessions = await _sessionService.GetSessionsForUserAsync(ownerId);
            return Ok(sessions);
        }
        catch (UserNotFoundException e)
        {
            _logger.LogWarning(e, "User with ID: {UserId} was not found", ownerId);
            return NotFound("User was not found");
        }
    }
}