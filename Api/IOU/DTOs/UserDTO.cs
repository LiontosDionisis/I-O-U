using Api.IOU.Models;

namespace Api.IOU.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public AvatarType Avatar { get; set; } = AvatarType.Penguin;
}