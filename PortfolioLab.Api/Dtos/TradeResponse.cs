using PortfolioLab.Domain.Trades;

namespace PortfolioLab.Api.Dtos
{
    public class TradeResponse
    {
        public required Guid TradeId { get; init; }
        public required string InstrumentId { get; init; }
        public required TradeSide Side { get; init; }
        public required decimal Quantity { get; init; }
        public required decimal UnitPrice { get; init; }
        public required DateTimeOffset ExecutedAt { get; init; }
    }
}
