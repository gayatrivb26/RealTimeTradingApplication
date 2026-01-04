using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Infrastructure.Data
{
    public class TradingDbContext: IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public TradingDbContext(DbContextOptions<TradingDbContext> options): base(options) { }

        // Identity
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        
        //public DbSet<User> users => Set<User>();
        public DbSet<Portfolio> Portfolios => Set<Portfolio>();
        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Trade> Trades => Set<Trade>();
        public DbSet<Holding> Holdings  => Set<Holding>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add Unique constraints or relationships

            // Application User Entity
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.Balance).HasPrecision(18, 4);
                entity.HasMany(u => u.Portfolios).WithOne(p => p.User)
                        .HasForeignKey(p => p.UserId)
                        .OnDelete(DeleteBehavior.Restrict);
            });

            // Stock entity
            modelBuilder.Entity<Stock>()
                .Property(s => s.CurrentPrice)
                .HasPrecision(18, 4);
            modelBuilder.Entity<Stock>()
                .HasIndex(s => s.Symbol)
                .IsUnique();
            modelBuilder.Entity<Stock>()
                .HasMany(s => s.Holdings)
                .WithOne(h => h.Stock)
                .HasForeignKey(h => h.StockId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent stock deletion if holdings exist


            // User entity
            //modelBuilder.Entity<User>()
            //    .Property(u => u.Balance)
            //    .HasPrecision(18, 2);
            //modelBuilder.Entity<User>()
            //    .HasMany(p => p.Portfolios)
            //    .WithOne(p => p.User)
            //    .HasForeignKey(p => p.UserId)
            //    .OnDelete(DeleteBehavior.Restrict); // Cascade delete portfolios when user is deleted

            // Portfolio entity
            modelBuilder.Entity<Portfolio>()
                .HasMany(p => p.Holdings)
                .WithOne(p => p.Portfolio)
                .HasForeignKey(h => h.PortfolioId)
                .OnDelete(DeleteBehavior.Restrict);


            // Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(s => s.Price).HasPrecision(18, 6);
                entity.Property(s => s.Quantity).HasPrecision(18, 6);
                entity.Property(s => s.RemainingQuantity).HasPrecision(18, 6);
                entity.HasOne(o => o.Portfolio).WithMany(p => p.Orders).HasForeignKey(o => o.PortfolioId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(o => o.User).WithMany().HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Restrict);
            });

            // Trade entity
            modelBuilder.Entity<Trade>()
                .Property(s => s.Quantity)
                .HasPrecision(18, 6);
            modelBuilder.Entity<Trade>()
                .Property(s => s.Price)
                .HasPrecision(18, 6);

            // Holding entity
            modelBuilder.Entity<Holding>()
                .Property(s => s.Quantity)
                .HasPrecision(18, 6);
            modelBuilder.Entity<Holding>()
                .Property(s => s.AverageCost)
                .HasPrecision(18, 6);

            // Refresh token entity
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasOne(rt => rt.User)
                      .WithMany()
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            // Seed the sample stocks
            modelBuilder.Entity<Stock>().
                HasData(
                    new Stock 
                    { 
                        Id = new Guid("11111111-1111-1111-1111-111111111111"), 
                        Symbol = "AAPL", 
                        CompanyName = "Apple Inc.", 
                        Sector = "Technology", 
                        CurrentPrice = 200.00m 
                    },
                    new Stock 
                    { 
                        Id = new Guid("22222222-2222-2222-2222-222222222222"), 
                        Symbol = "JPM", 
                        CompanyName = "JP Morgan Chase", 
                        Sector = "Finance", 
                        CurrentPrice = 150.00m 
                    }
                );
        }


    }
}
