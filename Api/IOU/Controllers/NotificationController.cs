using System.Security.Claims;
using Api.IOU.Exceptions;
using Api.IOU.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Api.IOU.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{

    private readonly INotificationService _notficiationService;
    private readonly INotificationSessionService _notSessionService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(INotificationService notifiationService, ILogger<NotificationController> logger, INotificationSessionService notSessionService)
    {
        _notficiationService = notifiationService;
        _logger = logger;
        _notSessionService = notSessionService;
    }

    [HttpGet("s/all")]
    public async Task<IActionResult> GetAllSessionNotificationsAsync(int userId)
    {
        var id = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            var notifications = await _notSessionService.GetAllAsync(id);
            return Ok(notifications);
        }
        catch (NoNotificationsException)
        {
            return NotFound(new { message = "You have no notifications" });
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { message = "User does not exist" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Internal Server Error. Please try again later." });
        }
    }

    [HttpDelete("s/{id:int}")]
    public async Task<IActionResult> DeleteSessionNotificationAsync(int id)
    {
        try
        {
            await _notSessionService.DeleteAsync(id);
            return Ok(new { message = "Notification deleted!" });
        }
        catch (NotificationDoesNotExistException)
        {
            return NotFound(new { message = "Notification does not exist" });
        }
        catch (Exception)
        {
            return StatusCode(500, new {message = "Internal Server Error. Please try again later."});
        }
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllAsync(int userId)
    {
        var id = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            var notifications = await _notficiationService.GetAllAsync(id);
            
            return Ok(notifications);
        }
        catch (NoNotificationsException)
        {
            return NotFound(new { message = "You have no notifications" });
        }
        catch (UserNotFoundException e)
        {
            _logger.LogWarning(e.Message, "User does not exist");
            return NotFound(new { message = "User does not exist." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Internal Server Error. Please try again later." });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var notification = await _notficiationService.GetByIdAsync(id);
            return Ok(notification);
        }
        catch (NotificationDoesNotExistException)
        {
            return NotFound($"Notification with ID: {id} does not exist");
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Internal Server Error. Please try again later." });
        }
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            await _notficiationService.DeleteAsync(id);
            return Ok(new { message = "Notification deleted!" });
        }
        catch (NotificationDoesNotExistException)
        {
            return NotFound("Notification does not exist");
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Internal Server Error. Please try again later." });
        }
    }
}