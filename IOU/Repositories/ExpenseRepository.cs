using Api.IOU.Data;
using Api.IOU.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.IOU.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _context;


    public ExpenseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        return expense;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null) return false;

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        return await _context.Expenses.Include(e => e.Splits).FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Expense>> GetExpensesBySessionIdAsync(int sessionId)
    {
        return await _context.Expenses.Include(s => s.Splits).Where(s => s.SessionId == sessionId).ToListAsync();
    }

    public async Task<Expense> UpdateAsync(Expense expense)
    {
        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();
        return expense;
    }
}