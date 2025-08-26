using Api.IOU.DTOs;

namespace Api.IOU.Services;

public interface IUserService
{
    Task<string> LoginUser(string username, string password);
    Task<UserDTO> RegisterAsync(UserRegisterDTO dto);
    Task<UserDTO?> GetByIdAsync(int id);
    Task<UserDTO?> GetByUsernameAsync(string username);
    Task<IEnumerable<UserDTO>> GetAllAsync();
    Task<UserDTO> UpdateAsync(int id, UserUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
}