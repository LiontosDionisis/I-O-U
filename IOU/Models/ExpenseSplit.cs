namespace Api.IOU.Models;

using System.ComponentModel.DataAnnotations;

public class ExpenseSplit
{
    public int Id { get; set; }

    [Required]
    public int ExpenseId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public SplitStatus Status { get; set; } = SplitStatus.Pending;

    // Navigation properties
    public Expense Expense { get; set; } = null!;
    public User User { get; set; } = null!;
}
