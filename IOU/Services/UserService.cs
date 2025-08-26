using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.IOU.Data;
using Api.IOU.DTOs;
using Api.IOU.Exceptions;
using Api.IOU.Helper;
using Api.IOU.Models;
using Microsoft.IdentityModel.Tokens;

namespace Api.IOU.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;
    private readonly IConfiguration _configuration;

    public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var toDelete = await _unitOfWork.Users.GetById(id);
        if (toDelete == null)
        {
            _logger.LogWarning("User with ID: {Id} was not found", id);
            throw new UserNotFoundException($"User with ID: {id} was not found.");
        }

        var deleted = await _unitOfWork.Users.DeleteAsync(toDelete.Id);

        if (deleted)
        {
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("User with ID: {Id} deleted successfully.", id);
        }
        else
        {
            _logger.LogError("Failed to delete user with ID: {Id}", id);
        }

        return deleted;
    }

    public async Task<IEnumerable<UserDTO>> GetAllAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return users.Select(u => ToUserDto(u));
    }

    public async Task<UserDTO?> GetByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetById(id);
        if (user == null)
        {
            _logger.LogWarning("User with id {UserId} not found", id);
            throw new UserNotFoundException($"User with ID: {id} was not found");
        }

        return ToUserDto(user);
    }

    public async Task<UserDTO?> GetByUsernameAsync(string username)
    {
        var user = await _unitOfWork.Users.GetByUsername(username);
        if (user == null)
        {
            _logger.LogWarning("User with username {Username} not found.", username);
            throw new UserNotFoundException($"User with username: {username} was not found.");
        }

        return ToUserDto(user);
    }

    public async Task<UserDTO> RegisterAsync(UserRegisterDTO dto)
    {

        //Check if username is registered
        var existingUsername = await _unitOfWork.Users.GetByUsername(dto.Username);
        if (existingUsername != null)
        {
            throw new UserAlreadyExistsException("Username already exists");
        }

        //Check if email is registered
        var existingEmail = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
        if (existingEmail != null)
        {
            throw new EmailAlreadyExistsException("Email already exists");
        }
        
        if (!PasswordHelper.IsValid(dto.Password))
        {
            _logger.LogWarning("Registration failed: weak password for username {Username}", dto.Username);
            throw new WeakPasswordException("Password must be at least 8 characters, include 1 uppercase letter and 1 special character.");
        }

        

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = PasswordHelper.HashPassword(dto.Password)
        };

        await _unitOfWork.Users.CreateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {Username} registered successfully at {Time}", user.Username, DateTime.UtcNow);

        return ToUserDto(user);

    }

    public async Task<UserDTO> UpdateAsync(int id, UserUpdateDTO dto)
    {
        var user = await _unitOfWork.Users.GetById(id);
        if (user == null)
        {
            _logger.LogWarning("User with id {UserId} not found for update.", id);
            throw new UserNotFoundException($"User with id {id} not found");
        }

        // Check if username is already in use
        var existingUsername = await _unitOfWork.Users.GetByUsername(dto.Username);
        if (existingUsername != null && existingUsername.Id != id)
        {
            _logger.LogWarning("Username {Username} is already taken", dto.Username);
            throw new UserAlreadyExistsException($"Username {dto.Username} is already taken");
        }

        // Check if email is already in use
        var existingEmail = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
        if (existingEmail != null && existingEmail.Id != id)
        {
            _logger.LogWarning("Email {Email} is already in use", dto.Email);
            throw new EmailAlreadyExistsException($"Email {dto.Email} is already in use");
        }
        

        user.Username = dto.Username;
        user.Email = dto.Email;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return ToUserDto(user);
    }

    private static UserDTO ToUserDto(User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<string> LoginUser(string username, string password)
    {
        var user = await _unitOfWork.Users.GetByUsername(username);
        if (user == null || !PasswordHelper.VerifyPassword(password, user.Password))
        {
            _logger.LogWarning("Login failed for {Username}", username);
            throw new InvalidLoginException("Invalid username or password");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            }),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        _logger.LogInformation("User {Username} logged in successfully at {Time}", username, DateTime.UtcNow);

        return jwtToken;
    }
}