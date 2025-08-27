namespace Api.IOU.DTOs;

public class BalanceDTO
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public decimal Paid { get; set; }
    public decimal Owes { get; set; }
    public decimal Balance => Paid - Owes;
}