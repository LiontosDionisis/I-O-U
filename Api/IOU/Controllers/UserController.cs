using Microsoft.AspNetCore.Mvc;
using Api.IOU.Services;
using Microsoft.AspNetCore.Authorization;
using Api.IOU.DTOs;
using Api.IOU.Exceptions;
using System.Xml;
using System.Security.Claims;

namespace Api.IOU.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var userDto = await _userService.GetByIdAsync(userId);

            return Ok(userDto);
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { message = "User does not exist." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Internal Server Error. Please try again later." });
        }




    }
    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }
        catch (UserNotFoundException e)
        {
            _logger.LogWarning(e, "User with ID {Id} not found", id);
            return NotFound(e.Message);
        }
    }

    [HttpGet("byUsername/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        try
        {
            var user = await _userService.GetByUsernameAsync(username);
            return Ok(user);
        }
        catch (UserNotFoundException e)
        {
            _logger.LogWarning(e, "User with username {Username} not found", username);
            return NotFound(e.Message);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDTO dto)
    {
        try
        {
            var user = await _userService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (UserAlreadyExistsException e)
        {
            _logger.LogWarning(e, "Registration failed: username already exists");
            return Conflict(new { message = "Username already exists.", code = "USER_ALREADY_EXISTS" });
        }
        catch (EmailAlreadyExistsException e)
        {
            _logger.LogWarning(e, "Registration failed: email already exists");
            return Conflict(new { message = "Email is already in use.", code = "EMAIL_IN_USE" });

        }
        catch (WeakPasswordException e)
        {
            _logger.LogWarning(e, "Registration failed: weak password");
            return BadRequest(new { message = "Password must be at least 8 characters long, have 1 capitalized letter and 1 special character.", code = "WEAK_PASSWORD" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during registration");
            return StatusCode(500, new { message = "An unexpected error occured. Please try again later.", code = "SERVER_ERROR" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        try
        {
            var token = await _userService.LoginUser(dto.Username, dto.Password);
            return Ok(new { Token = token });
        }
        catch (InvalidLoginException e)
        {
            _logger.LogWarning(e, "Login failed for {Username}", dto.Username);
            return Unauthorized(new { message = "Invalid username or password", code = "INVALID_LOGIN_ERROR" });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _userService.DeleteAsync(id);
            _logger.LogInformation("User with ID: {Id} was deleted", id);
            return Ok(new { message = "User deleted successfully" });
        }
        catch (UserNotFoundException e)
        {
            _logger.LogWarning(e, "User with ID: {Id} was not found", id);
            return NotFound(new { message = "User not found", code = "USER_NOT_FOUND" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during registration");
            return StatusCode(500, new { message = "An unexpected error occured. Please try again later.", code = "SERVER_ERROR" });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDTO dto)
    {
        try
        {
            var updatedUser = await _userService.UpdateAsync(id, dto);
            return Ok(new { message = "User updated!", user = updatedUser });
        }
        catch (UserNotFoundException e)
        {
            _logger.LogWarning(e, "User with ID {Id} not found for update", id);
            return NotFound(new { message = "User not found", code = "USER_NOT_FOUND" });
        }
        catch (UserAlreadyExistsException e)
        {
            _logger.LogWarning(e, "User with username {Username} already exists", dto.Username);
            return Conflict(new { message = "Username is already taken", code = "USER_ALREADY_EXISTS" });
        }
        catch (EmailAlreadyExistsException e)
        {
            _logger.LogWarning(e, "Email {Email} already in use", dto.Email);
            return Conflict(new { message = "Email already in use", code = "EMAIL_EXISTS" });
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Unexpected error while updating user with ID {Id}", id);
            return StatusCode(500, new { message = "An unexpected error has occured. Please try again later.", code = "INTERNAL_SERVER_ERROR" });
        }
    }

    [HttpPatch("update-username/{id:int}")]
    public async Task<IActionResult> UpdateUsername(int id, [FromBody] UserUpdateUsernameDTO dto)
    {
        try
        {
            var updatedUser = await _userService.UpdateUsernameAsync(id, dto);
            return Ok(updatedUser);
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { message = "User does not exist" });
        }
        catch (UserAlreadyExistsException)
        {
            return Conflict(new { message = "Username is already in use." });
        }

    }

    [HttpPatch("update-email/{id:int}")]
    public async Task<IActionResult> UpdateEmail(int id, [FromBody] UserUpdateEmailDTO dto)
    {
        try
        {
            var updatedUser = await _userService.UpdateEmailAsync(id, dto);
            return Ok(updatedUser);
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { message = "User does not exist." });
        }
        catch (EmailAlreadyExistsException)
        {
            return Conflict(new { message = "Email is already in use" });
        }
    }
}