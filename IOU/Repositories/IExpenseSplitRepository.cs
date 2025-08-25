using Api.IOU.Models;

namespace Api.IOU.Repositories;

public interface IExpenseSplitRepository
{
    Task<ExpenseSplit> CreateAsync(ExpenseSplit expenseSplit);
    Task<IEnumerable<ExpenseSplit>> GetSplitsByExpenseId(int expenseId);
    Task<bool> DeleteAsync(int id);
}