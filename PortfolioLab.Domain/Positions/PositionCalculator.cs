using PortfolioLab.Domain.Trades;

namespace PortfolioLab.Domain.Positions;

public sealed class PositionCalculator
{
    public IReadOnlyCollection<Position> CalculatePositions(List<Trade> trades)
    {
        Trade trade = trades.Single();
        Position position = new Position 
        { 
            InstrumentId = trade.InstrumentId,
            Quantity = trade.Quantity,
            AverageCost = trade.UnitPrice,
            RealizedProfitLoss = 0m 
        };
        return new List<Position> { position };
    }
}