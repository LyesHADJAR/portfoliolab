using PortfolioLab.Domain.Trades;
using Microsoft.EntityFrameworkCore;

namespace PortfolioLab.Infrastructure.Persistence
{
    public class PortfolioDbContext : DbContext
    {
        public DbSet<Trade> Trades { get; set; } = null!;

        public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options ) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trade>(entity =>
            {
                entity.Property(t => t.Quantity).HasPrecision(18, 8);
                entity.Property(t => t.UnitPrice).HasPrecision(18, 8);
            });
        }
    }
}
