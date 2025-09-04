using Api.IOU.Repositories;

namespace Api.IOU.Data;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IFriendshipRepository Friendships { get; }
    ISessionRepository Sessions { get; }
    ISessionUserRepository SessionUsers { get; }
    IExpenseRepository Expenses { get; }
    IExpenseSplitRepository ExpenseSplits { get; }
    INotificationRepository Notifications { get; }
    INotificationSessionRepository SessionNotifications {get;}

    Task<int> SaveChangesAsync();
}