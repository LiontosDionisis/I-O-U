using Api.IOU.Data;
using Api.IOU.Exceptions;
using Api.IOU.Models;

namespace Api.IOU.Services;

public class ExpenseService : IExpenseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExpenseService> _logger;

    public ExpenseService(IUnitOfWork unitOfWork, ILogger<ExpenseService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task<Expense> CreateExpenseAsync(int sesisonId, int paidById, decimal totalAmount, string description, bool splitEqually, Dictionary<int, decimal>? customSplits = null)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteExpenseAsync(int expenseId, int userId)
    {
        var existingExpense = await _unitOfWork.Expenses.GetByIdAsync(expenseId);
        if (existingExpense == null) throw new ExpenseNotFoundException("Expense does not exist");
        if (existingExpense.PaidById != userId) throw new UnauthorizedAccessException("Only the payer can delete an expense");

        await _unitOfWork.Expenses.DeleteAsync(expenseId);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public Task<IEnumerable<Expense>> GetExpensesBySessionIdAsync(int sessionId)
    {
        throw new NotImplementedException();
    }
}