using Api.IOU.Data;
using Api.IOU.Models;

namespace Api.IOU.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _context;

//TODO
    public ExpenseRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Expense> CreateAsync(Expense expense)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Expense?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Expense>> GetExpensesBySessionIdAsync(int sessionId)
    {
        throw new NotImplementedException();
    }

    public Task<Expense> UpdateAsync(Expense expense)
    {
        throw new NotImplementedException();
    }
}