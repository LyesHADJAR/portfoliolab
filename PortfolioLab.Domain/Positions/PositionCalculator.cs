using PortfolioLab.Domain.Trades;

namespace PortfolioLab.Domain.Positions;

public sealed class PositionCalculator
{
    public IReadOnlyCollection<Position> CalculatePositions(List<Trade> trades)
    {
        decimal totalQuantity = 0m;
        decimal totalCost = 0m;

        foreach(Trade currentTrade in trades)
        {
            totalQuantity += currentTrade.Quantity;
            totalCost += currentTrade.Quantity * currentTrade.UnitPrice;
        }

        decimal  averageCost = totalCost / totalQuantity;

        Trade firstTrade = trades.First();
        Position position = new Position 
        { 
            InstrumentId = firstTrade.InstrumentId,
            Quantity = totalQuantity,
            AverageCost = averageCost,
            RealizedProfitLoss = 0m 
        };
        return new List<Position> { position };
    }
}