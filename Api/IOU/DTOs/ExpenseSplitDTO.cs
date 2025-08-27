using Api.IOU.Models;

namespace Api.IOU.DTOs;

public class ExpenseSplitDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public SplitStatus Status { get; set; } = SplitStatus.Pending;
}