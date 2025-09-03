namespace Api.IOU.DTOs;

public class FriendNotificationDTO {
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int? FriendshipId { get; set; }
}