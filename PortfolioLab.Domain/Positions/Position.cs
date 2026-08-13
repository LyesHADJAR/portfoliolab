namespace PortfolioLab.Domain.Positions;

public sealed class Position
{
    public required string InstrumentId { get; init; }
    public required decimal Quantity { get; init; }
    public required decimal AverageCost { get; init; }
    public required decimal RealizedProfitLoss { get; init; }
}