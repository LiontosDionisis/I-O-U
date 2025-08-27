using System.Security.Claims;
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
    public async Task<IActionResult> CreateSession([FromBody] string name)
    {
        var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            var session = await _sessionService.CreateSessionAsync(userId, name);
            return Ok(session);
        }
        catch (Exception e)
        {
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
            return NotFound(new { message = "Session was not found or does not exist." });
        }
        catch (UserNotFoundException e)
        {
            return NotFound(new { message = "User was not found." });
        }
        catch (CannotAddUserToSessionException e)
        {
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
            return NotFound("Session not found");
        }
        catch (UserNotFoundException e)
        {
            return NotFound("User was not found");
        }
        catch (InvalidOperationException e)
        {
            return BadRequest("Cannot remove yourself from session");
        }
        catch (UnauthorizedAccessException e)
        {
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
            return NotFound("User was not found");
        }
    }
}