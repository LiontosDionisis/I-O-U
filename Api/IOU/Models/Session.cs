using System.ComponentModel.DataAnnotations;
using api.IOU.Models;

namespace Api.IOU.Models;

public class Session
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int CreatedById { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User CreatedBy { get; set; } = null!;
    public ICollection<SessionUser> Participants { get; set; } = new List<SessionUser>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}