namespace PortfolioLab.Domain.Trades;

public sealed class Trade
{
    public required Guid TradeId { get; init; }
    public required string InstrumentId { get; init; }
    public required TradeSide Side { get; init; }
    public required decimal Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
    public required DateTimeOffset ExecutedAt { get; init; }
}