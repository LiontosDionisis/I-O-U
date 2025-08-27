namespace Api.IOU.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Expense
{
    public int Id { get; set; }

    [Required]
    public int SessionId { get; set; }

    [Required]
    public int PaidById { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Session Session { get; set; } = null!;
    public User PaidBy { get; set; } = null!;
    public ICollection<ExpenseSplit> Splits { get; set; } = new List<ExpenseSplit>();
}
