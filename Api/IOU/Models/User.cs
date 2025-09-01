using System.ComponentModel.DataAnnotations;
using api.IOU.Models;

namespace Api.IOU.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(225)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(255)]
    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter and one special character.")]
    public string Password { get; set; } = string.Empty;

    public ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();
    public ICollection<SessionUser> Sessions { get; set; } = new List<SessionUser>();
    public ICollection<Expense> ExpensesPaid { get; set; } = new List<Expense>();
    public ICollection<ExpenseSplit> ExpenseSplits { get; set; } = new List<ExpenseSplit>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

