using Api.IOU.Models;

namespace Api.IOU.Services;

public interface IExpenseService
{
    Task<Expense> CreateExpenseAsync(int sesisonId, int paidById, decimal totalAmount, string description, bool splitEqually, Dictionary<int, decimal>? customSplits = null);

    Task<IEnumerable<Expense>> GetExpensesBySessionIdAsync(int sessionId);
    Task<bool> DeleteExpenseAsync(int expenseId, int userId);
    

}