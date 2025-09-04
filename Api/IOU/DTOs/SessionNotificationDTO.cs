using Api.IOU.Models;

namespace Api.IOU.DTOs;

public class SessionNotificationDTO
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int SessionId { get; set; }
    
}