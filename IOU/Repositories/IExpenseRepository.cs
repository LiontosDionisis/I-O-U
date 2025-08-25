using Api.IOU.Models;

namespace Api.IOU.Repositories;

public interface IExpenseRepository
{
    Task<Expense> CreateAsync(Expense expense);
    Task<Expense?> GetByIdAsync(int id);
    Task<IEnumerable<Expense>> GetExpensesBySessionIdAsync(int sessionId);
    Task<Expense> UpdateAsync(Expense expense);
    Task<bool> DeleteAsync(int id);
}