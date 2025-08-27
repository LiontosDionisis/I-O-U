using Api.IOU.DTOs;
using Api.IOU.Models;

namespace Api.IOU.Services;

public interface IExpenseService
{
    Task<ExpenseDTO> CreateExpenseAsync(AddExpenseDTO dto);

    Task<IEnumerable<ExpenseDTO>> GetExpensesBySessionIdAsync(int sessionId);
    Task<bool> DeleteExpenseAsync(int expenseId, int userId);
    Task<ExpenseSplit> SettleExpenseSplitAsync(int expenseId, int userId);
    Task<IEnumerable<BalanceDTO>> GetSessionBalancesAsync(int sessionId);

}