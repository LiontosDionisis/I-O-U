using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.IOU.Data;
using Api.IOU.DTOs;
using Api.IOU.Helper;
using Api.IOU.Models;
using Microsoft.IdentityModel.Tokens;

namespace Api.IOU.Services;

public class UserService : IUerService
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
        var deleted = await _unitOfWork.Users.DeleteAsync(id);
        if (deleted)
        {
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("User with Id {UserId} deleted successfully!", id);
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
        try
        {
            var user = await _unitOfWork.Users.GetById(id);
            return user == null ? null : ToUserDto(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching user with id {Userid}", id);
            throw;
        }
    }

    public async Task<UserDTO?> GetByUsernameAsync(string username)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByUsername(username);
            return user == null ? null : ToUserDto(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching user with username {Username}", username);
            throw;
        }
    }

    public async Task<UserDTO> RegisterAsync(UserRegisterDTO dto)
    {

        //Check if username is registered
        var existingUsername = await _unitOfWork.Users.GetByUsername(dto.Username);
        if (existingUsername != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        //Check if email is registered
        var existingEmail = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
        if (existingEmail != null)
        {
            throw new InvalidOperationException("Email already exists");
        }
        
        if (!PasswordHelper.IsValid(dto.Password))
        {
            _logger.LogWarning("Registration failed: weak password for username {Username}", dto.Username);
            throw new ArgumentException("Password must be at least 8 characters, include 1 uppercase letter and 1 special character.");
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

    public async Task<UserDTO> UpdateAsync(UserDTO dto)
    {
        var user = await _unitOfWork.Users.GetById(dto.Id);
        if (user == null)
        {
            _logger.LogWarning("User with id {UserId} not found for update.", dto.Id);
            throw new KeyNotFoundException($"User with id {dto.Id} not found");
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
            throw new UnauthorizedAccessException("Invalid username or password");
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