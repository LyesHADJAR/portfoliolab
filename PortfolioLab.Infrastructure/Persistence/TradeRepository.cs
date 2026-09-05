using PortfolioLab.Domain.Trades;
using Microsoft.EntityFrameworkCore;

namespace PortfolioLab.Infrastructure.Persistence
{
    public class TradeRepository
    {
        private readonly PortfolioDbContext _dbcontext;
        public TradeRepository(PortfolioDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Trade> AddAsync(Trade trade)
        {
            await _dbcontext.Trades.AddAsync(trade);
            await _dbcontext.SaveChangesAsync();
            return trade;
        }

        public async Task<IReadOnlyCollection<Trade>> GetAllAsync()
        {
            return await _dbcontext.Trades.OrderBy(t => t.ExecutedAt).ToListAsync();

        }
    }
}
