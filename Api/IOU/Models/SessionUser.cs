using System.ComponentModel.DataAnnotations;
using Api.IOU.Models;

namespace api.IOU.Models;

public class SessionUser
{
    [Required]
    public int SessionId { get; set; }

    [Required]
    public int UserId { get; set; }

    public Session Session { get; set; } = null!;
    public User User { get; set; } = null!;
}