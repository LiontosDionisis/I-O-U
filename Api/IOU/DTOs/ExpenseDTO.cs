namespace Api.IOU.DTOs;

public class ExpenseDTO
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int PaidById { get; set; }
    public decimal TotalAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<ExpenseSplitDTO> Splits { get; set; } = new List<ExpenseSplitDTO>();
}