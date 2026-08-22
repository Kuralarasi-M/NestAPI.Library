using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection.Emit;

namespace LibraryApp.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> option) : base(option)
        { }
        public DbSet<User> Users => Set<User>();

        public DbSet<Book> Books => Set<Book>();

        public DbSet<Favorite> Favorites => Set<Favorite>();

        public DbSet<ReadingHistory> ReadingHistories => Set<ReadingHistory>();

        public DbSet<DownloadHistory> DownloadHistories => Set<DownloadHistory>();

        public DbSet<SearchHistory> SearchHistories => Set<SearchHistory>();
        public DbSet<UserFcmToken> UserFcmTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Relationships
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<ReadingHistory>()
                .HasOne(r => r.User)
                .WithMany(u => u.ReadingHistories)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<SearchHistory>()
                .HasOne(s => s.User)
                .WithMany(u => u.SearchHistories)
                .HasForeignKey(s => s.UserId);
            modelBuilder.Entity<UserFcmToken>()
    .HasOne(x => x.User)
    .WithMany()
    .HasForeignKey(x => x.UserId)
    .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
