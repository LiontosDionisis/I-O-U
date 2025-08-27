using Api.IOU.Models;

namespace Api.IOU.DTOs;

public class AddExpenseDTO
{
    public int SessionId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int PaidById { get; set; }
    public bool IsEqualSplit { get; set; }
    public List<ExpenseSplitDTO>? CustomSplits { get; set; } = null;
}