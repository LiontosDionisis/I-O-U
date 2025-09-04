using System.Security.Claims;
using Api.IOU.DTOs;
using Api.IOU.Exceptions;
using Api.IOU.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.IOU.Controllers;

[ApiController]
[Route("api/friendship")]
[Authorize]
public class FriendshipController : ControllerBase
{
    private readonly IFriendshipService _friendshipService;
    private readonly ILogger<FriendshipController> _logger;

    public FriendshipController(IFriendshipService friendshipService, ILogger<FriendshipController> logger)
    {
        _friendshipService = friendshipService;
        _logger = logger;
    }

    [HttpPost("{friendId:int}")]
    public async Task<IActionResult> SendFriendRequest(int friendId)
    {
        var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value); // userId in JWT Token

        try
        {
            var friendship = await _friendshipService.SendFriendRequestAsync(userId, friendId);
            _logger.LogInformation("Friend request send by user with iD {userId} to User with ID {friendID}", userId, friendId);

            var dto = new FriendshipDTO
            {
                Id = friendship.Id,
                FriendId = friendship.FriendId,
                Status = friendship.Status
            };

            return Ok(new { message = "Friend request sent!", friendship = dto });
        }
        catch (InvalidFriendRequest e)
        {
            _logger.LogWarning(e, "Failed to send friend request from {UserId} to {FriendId}", userId, friendId);
            return BadRequest(new { message = "Failed to send friend request" });
        }

    }

    [HttpPut("accept/{requestId:int}")]
    public async Task<IActionResult> AcceptFriendRequest(int requestId)
    {
        var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        try
        {
            var friendship = await _friendshipService.AcceptFriendRequestAsync(requestId, userId);
            _logger.LogInformation("Friend request accepted from user {userId}", userId);

            return Ok(new { message = "Friend request accepted"});
        }
        catch (FriendRequestNotFoundException e)
        {
            _logger.LogWarning(e, " Friend request was not found");
            return NotFound(new { message = "Friend request not found" });
        }
        catch (InvalidFriendRequest e)
        {
            _logger.LogWarning(e, "Invalid friend request.");
            return BadRequest(new { message = "Invalid friend request." });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error has occured.");
            return StatusCode(500, new { message = "An unexpected error has occured. Please try again later" });
        }
    }

    [HttpDelete("deny/{requestId:int}")]
    public async Task<IActionResult> DenyFriendRequest(int requestId)
    {
        var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        try
        {
            var friendship = await _friendshipService.DenyFriendRequestAsync(requestId, userId);
            _logger.LogInformation("Friend request denied from user {userId}", userId);

            return Ok(new { message = "Friend request denied", success = friendship });
        }
        catch (FriendRequestNotFoundException e)
        {
            _logger.LogWarning(e, " Friend request was not found");
            return NotFound(new { message = "Friend request not found" });
        }
        catch (InvalidFriendRequest e)
        {
            _logger.LogWarning(e, "Invalid friend request.");
            return BadRequest(new { message = "Invalid friend request." });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error has occured.");
            return StatusCode(500, new { message = "An unexpected error has occured. Please try again later" });
        }
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        var pendingRequests = await _friendshipService.GetPendingRequestsAsync(userId);
        return Ok(pendingRequests);
    }

    [HttpGet("friends")]
    public async Task<IActionResult> GetFriends()
    {
        var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        var friends = await _friendshipService.GetFriendsAsync(userId);
        return Ok(friends);
    }
}