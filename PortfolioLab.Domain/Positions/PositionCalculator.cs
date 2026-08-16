using PortfolioLab.Domain.Trades;
using System.Security;

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
            decimal realizedProfitLoss = 0m;

            foreach(Trade trade in group)
            {
                if (trade.Side == TradeSide.Buy)
                {
                    totalQuantity += trade.Quantity;
                    totalCost += trade.Quantity * trade.UnitPrice;
                }
                else if (trade.Side == TradeSide.Sell)
                {
                    decimal currentAverageCost = totalCost / totalQuantity;
                    realizedProfitLoss += (trade.UnitPrice - currentAverageCost) * trade.Quantity;
                    totalQuantity -= trade.Quantity;
                    totalCost -= trade.Quantity * currentAverageCost;

                }

            }

            decimal averageCost = totalCost / totalQuantity;

            Position position = new Position
            {
                InstrumentId = group.Key,
                Quantity = totalQuantity,
                AverageCost = averageCost,
                RealizedProfitLoss = realizedProfitLoss
            };
            positions.Add(position);
        }


        return positions;
    }
}