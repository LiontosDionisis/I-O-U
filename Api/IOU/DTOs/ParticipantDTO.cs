using Api.IOU.Models;

namespace Api.IOU.DTOs;

public class ParticipantDTO
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public AvatarType Avatar { get; set; } = AvatarType.Penguin;
}