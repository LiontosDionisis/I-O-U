namespace Api.IOU.DTOs;

public class NotificationDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    
}