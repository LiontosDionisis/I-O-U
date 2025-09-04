using System.ComponentModel.DataAnnotations.Schema;

namespace Api.IOU.Models;

[Table("notificationsession")]

public class NotificationSession
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User? User { get; set; }
    public int SessionId { get; set; }
    public Session? Session { get; set; }
}