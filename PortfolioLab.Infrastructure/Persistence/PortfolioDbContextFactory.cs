using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PortfolioLab.Infrastructure.Persistence
{
    public class PortfolioDbContextFactory
        : IDesignTimeDbContextFactory<PortfolioDbContext>
    {
        public PortfolioDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<PortfolioDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PortfolioLab;Trusted_Connection=true;")
                .Options;

            return new PortfolioDbContext(options);
        }
    }
}