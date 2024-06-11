using Microsoft.EntityFrameworkCore;
using financialApp.Models;

namespace financialApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserGroupMembership> UserGroupMemberships { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<SavingGoal> SavingGoals { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<PeriodicReport> PeriodicReports { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=FinancialDatabaseApp;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroupMembership>(entity =>
            {
                entity.HasKey(e => new { e.UserID, e.GroupID });

                entity.HasOne(ugm => ugm.User)
                      .WithMany(u => u.UserGroups)
                      .HasForeignKey(ugm => ugm.UserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ugm => ugm.UserGroup)
                      .WithMany(ug => ug.Users)
                      .HasForeignKey(ugm => ugm.GroupID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.UserID).ValueGeneratedOnAdd();
                entity.Property(e => e.Role).IsRequired();
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionID);
                entity.HasOne(t => t.User)
                      .WithMany(u => u.Transactions)
                      .HasForeignKey(t => t.UserID);
                entity.Property(t => t.Amount).HasColumnType("decimal(18,2)");

                // Konwersja enum na string
                entity.Property(t => t.Type)
                      .HasConversion(
                          v => v.ToString(),
                          v => (TransactionType)Enum.Parse(typeof(TransactionType), v))
                      .HasMaxLength(50); // Opcjonalnie, aby określić maksymalną długość pola string
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => e.GroupID);
            });

            modelBuilder.Entity<SavingGoal>(entity =>
            {
                entity.HasKey(e => e.GoalID);
                entity.HasOne(sg => sg.User)
                      .WithMany(u => u.SavingGoals)
                      .HasForeignKey(sg => sg.UserID);
                entity.Property(sg => sg.Amount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.HasKey(e => e.ReminderID);
                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reminders)
                      .HasForeignKey(r => r.UserID);
            });

            modelBuilder.Entity<PeriodicReport>(entity =>
            {
                entity.HasKey(e => e.ReportID);
                entity.HasOne(pr => pr.User)
                      .WithMany(u => u.PeriodicReports)
                      .HasForeignKey(pr => pr.UserID);
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.ItemID);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            });
        }
    }
}
