using Api.IOU.Data;
using Api.IOU.DTOs;
using Api.IOU.Exceptions;
using Api.IOU.Hubs;
using Api.IOU.Models;
using Microsoft.AspNetCore.SignalR;

namespace Api.IOU.Services;

public class ExpenseService : IExpenseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExpenseService> _logger;
    private readonly IHubContext<SessionHub> _hubContext;

    public ExpenseService(IUnitOfWork unitOfWork, ILogger<ExpenseService> logger, IHubContext<SessionHub> hubContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task<ExpenseDTO> CreateExpenseAsync(AddExpenseDTO dto)
    {
        var session = await _unitOfWork.Sessions.GetByIdAsync(dto.SessionId);
        if (session == null) throw new SessionNotFoundException("Session does not exist");

        if (!session.Participants.Any(p => p.UserId == dto.PaidById))
            throw new InvalidOperationException("Payer is not part of the session");

        var expense = new Expense
        {
            SessionId = dto.SessionId,
            PaidById = dto.PaidById,
            TotalAmount = dto.TotalAmount,
            Description = dto.Description
        };

        var splits = new List<ExpenseSplit>();

        if (dto.IsEqualSplit)
        {
            var participantIds = session.Participants.Select(p => p.UserId).ToList();
            var splitAmount = Math.Round(dto.TotalAmount / participantIds.Count, 2);

            splits.AddRange(participantIds.Select(userId => new ExpenseSplit
            {
                UserId = userId,
                Amount = splitAmount,
                Status = SplitStatus.Pending
            }));
        }
        else
        {
            if (dto.CustomSplits == null || !dto.CustomSplits.Any())
                throw new InvalidOperationException("Custom splits required for non-equal split");

            if (dto.CustomSplits.Sum(s => s.Amount) != dto.TotalAmount)
                throw new InvalidOperationException("Sum of splits does not equal total amount");

            foreach (var split in dto.CustomSplits)
            {
                if (!session.Participants.Any(p => p.UserId == split.UserId))
                    throw new InvalidOperationException("User is not part of the session");

                splits.Add(new ExpenseSplit
                {
                    UserId = split.UserId,
                    Amount = split.Amount,
                    Status = SplitStatus.Pending
                });
            }
        }

        expense.Splits = splits;

        await _unitOfWork.Expenses.CreateAsync(expense);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Expense with ID {ExpenseId} created in session {SessionId}", expense.Id, expense.SessionId);



        // Map to DTO
        var expenseDto = new ExpenseDTO
        {
            Id = expense.Id,
            SessionId = expense.SessionId,
            PaidById = expense.PaidById,
            TotalAmount = expense.TotalAmount,
            Description = expense.Description,
            Splits = expense.Splits.Select(s => new ExpenseSplitDTO
            {
                UserId = s.UserId,
                Amount = s.Amount,
                Status = s.Status
            }).ToList()
        };
        
        var participants = session.Participants.Select(p => p.UserId.ToString());
        await _hubContext.Clients.Users(participants).SendAsync("ExpenseCreated", expenseDto);

        return expenseDto;
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

    public async Task<IEnumerable<ExpenseDTO>> GetExpensesBySessionIdAsync(int sessionId)
    {
        var expenses = await _unitOfWork.Expenses.GetExpensesBySessionIdAsync(sessionId);

        return expenses.Select(expense => new ExpenseDTO
        {
            Id = expense.Id,
            SessionId = expense.SessionId,
            PaidById = expense.PaidById,
            TotalAmount = expense.TotalAmount,
            Description = expense.Description,
            Splits = expense.Splits.Select(s => new ExpenseSplitDTO
            {
                UserId = s.UserId,
                Amount = s.Amount,
                Status = s.Status
            }).ToList()
        });
    }

    public async Task<IEnumerable<BalanceDTO>> GetSessionBalancesAsync(int sessionId)
    {
        var session = await _unitOfWork.Sessions.GetByIdAsync(sessionId);
        if (session == null) throw new SessionNotFoundException("Session does not exist");

        var balances = new List<BalanceDTO>();

        foreach (var participant in session.Participants)
        {
            var user = participant.User;

            // Total paid by this user
            var paid = session.Expenses
                .Where(e => e.PaidById == user.Id)
                .Sum(e => e.TotalAmount);

            // Total owed by this user (only pending splits)
            var owes = session.Expenses
                .SelectMany(e => e.Splits)
                .Where(s => s.UserId == user.Id && s.Status == SplitStatus.Pending)
                .Sum(s => s.Amount);

            balances.Add(new BalanceDTO
            {
                UserId = user.Id,
                Username = user.Username,
                Paid = paid,
                Owes = owes
            });
        }

        return balances;
    }

    public async Task<ExpenseSplit> SettleExpenseSplitAsync(int expenseId, int userId)
    {
        var expense = await _unitOfWork.Expenses.GetByIdAsync(expenseId);
        if (expense == null)
            throw new ExpenseNotFoundException("Expense does not exist");


        var split = expense.Splits.FirstOrDefault(s => s.UserId == userId);
        if (split == null)
            throw new InvalidOperationException("User is not part of this expense");


        split.Status = SplitStatus.Settled;

        await _unitOfWork.ExpenseSplits.UpdateAsync(split);


        await _unitOfWork.SaveChangesAsync();

        return split;
    }

    

}