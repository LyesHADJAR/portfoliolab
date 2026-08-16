using PortfolioLab.Domain.Trades;

namespace PortfolioLab.Domain.Positions;

public sealed class PositionCalculator
{
    public IReadOnlyCollection<Position> CalculatePositions(List<Trade> trades)
    {
        List<Position> positions = new List<Position>();

        var groupedTrades = trades.GroupBy(trade => trade.InstrumentId);

        foreach(var group in groupedTrades)
        {
            decimal totalQuantity = 0m;
            decimal totalCost = 0m;

            foreach(Trade trade in group)
            {
                totalQuantity += trade.Quantity;
                totalCost += trade.Quantity * trade.UnitPrice;
            }

            decimal averageCost = totalCost / totalQuantity;

            Position position = new Position
            {
                InstrumentId = group.Key,
                Quantity = totalQuantity,
                AverageCost = averageCost,
                RealizedProfitLoss = 0m
            };
            positions.Add(position);
        }


        return positions;
    }
}