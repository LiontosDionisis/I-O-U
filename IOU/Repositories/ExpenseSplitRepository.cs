using Api.IOU.Data;
using Api.IOU.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.IOU.Repositories;

public class ExpenseSplitRepository : IExpenseSplitRepository
{
    private readonly AppDbContext _context;

    public ExpenseSplitRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseSplit> CreateAsync(ExpenseSplit expenseSplit)
    {
        _context.ExpenseSplits.Add(expenseSplit);
        await _context.SaveChangesAsync();
        return expenseSplit;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var split = await _context.ExpenseSplits.FindAsync(id);
        if (split == null) return false;

        _context.ExpenseSplits.Remove(split);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ExpenseSplit>> GetSplitsByExpenseId(int expenseId)
    {
        return await _context.ExpenseSplits.Include(s => s.User).Where(s => s.ExpenseId == expenseId).ToListAsync();
    }

    public async Task<ExpenseSplit> UpdateAsync(ExpenseSplit split)
{
    _context.ExpenseSplits.Update(split);
    await _context.SaveChangesAsync();
    return split;
}

}