using Api.IOU.Repositories;

namespace Api.IOU.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; }
    public IFriendshipRepository Friendships { get; }
    public ISessionRepository Sessions { get; }
    public ISessionUserRepository SessionUsers { get; }
    public IExpenseRepository Expenses { get; }
    public IExpenseSplitRepository ExpenseSplits { get; }
    public INotificationRepository Notifications { get; }
    public INotificationSessionRepository SessionNotifications{ get; }

    public UnitOfWork(AppDbContext context, IUserRepository users, IFriendshipRepository friendships, ISessionRepository sessions, ISessionUserRepository sessionUsers, IExpenseRepository expenses, IExpenseSplitRepository expenseSplits, INotificationRepository notifications, INotificationSessionRepository sessionNotifications)
    {
        _context = context;
        Users = users;
        Friendships = friendships;
        Sessions = sessions;
        SessionUsers = sessionUsers;
        Expenses = expenses;
        ExpenseSplits = expenseSplits;
        Notifications = notifications;
        SessionNotifications = sessionNotifications;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}