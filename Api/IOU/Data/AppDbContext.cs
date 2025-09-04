using Api.IOU.Models;
using api.IOU.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.IOU.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<SessionUser> Sessionusers { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpenseSplit> ExpenseSplits { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationSession> SessionNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SessionUser>()
            .HasKey(su => new { su.SessionId, su.UserId });

        modelBuilder.Entity<SessionUser>()
            .HasOne(su => su.Session)
            .WithMany(s => s.Participants)
            .HasForeignKey(su => su.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SessionUser>()
            .HasOne(su => su.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(su => su.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.User)
            .WithMany(u => u.Friendships)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.Friend)
            .WithMany()
            .HasForeignKey(f => f.FriendId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Friendship>()
            .Property(f => f.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();


        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Session)
            .WithMany(s => s.Expenses)
            .HasForeignKey(e => e.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.PaidBy)
            .WithMany(u => u.ExpensesPaid)
            .HasForeignKey(e => e.PaidById)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<ExpenseSplit>()
            .HasOne(es => es.Expense)
            .WithMany(e => e.Splits)
            .HasForeignKey(es => es.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExpenseSplit>()
            .HasOne(es => es.User)
            .WithMany(u => u.ExpenseSplits)
            .HasForeignKey(es => es.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExpenseSplit>()
            .Property(e => e.Status)
            .HasConversion<string>();


        modelBuilder.Entity<Session>()
            .HasOne(s => s.CreatedBy)
            .WithMany()
            .HasForeignKey(s => s.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(u => u.User)
            .WithMany(n => n.Notifications)
            .HasForeignKey(fk => fk.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(f => f.Friendship)
            .WithMany()
            .HasForeignKey(e => e.FriendshipId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NotificationSession>()
            .HasOne(s => s.Session)
            .WithMany()
            .HasForeignKey(fk => fk.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NotificationSession>()
            .HasOne(u => u.User)
            .WithMany(n => n.SessionNotifications)
            .HasForeignKey(fk => fk.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

 }