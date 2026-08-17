using PortfolioLab.Domain.Trades;

namespace PortfolioLab.Domain.Positions;

public sealed class PositionCalculator
{
    private static void ValidateTrades(IEnumerable<Trade> trades)
    {
        HashSet<Guid> seenTradeIds = new HashSet<Guid>();

        foreach (Trade trade in trades)
        {
            if (!seenTradeIds.Add(trade.TradeId))
            {
                throw new InvalidOperationException($"Duplicate trade detected: {trade.TradeId}");
            }

            if (trade.Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(trade.Quantity), trade.Quantity, "Trade quantity must be positive.");
            }

            if (trade.UnitPrice <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(trade.UnitPrice), trade.UnitPrice, "Trade unit price must be positive.");
            }
        }

    }

    public IReadOnlyCollection<Position> CalculatePositions(IEnumerable<Trade> trades)
    {
        
        List<Trade> tradeList = trades.ToList();
        ValidateTrades(tradeList);

        List<Position> positions = new List<Position>();

        var groupedTrades = tradeList
            .OrderBy(trade => trade.ExecutedAt)
            .GroupBy(trade => trade.InstrumentId);

        foreach (var group in groupedTrades)
        {
            decimal totalQuantity = 0m;
            decimal totalCost = 0m;
            decimal realizedProfitLoss = 0m;

            foreach (Trade trade in group)
            {

                switch (trade.Side)
                {
                    case TradeSide.Buy:
                        totalQuantity += trade.Quantity;
                        totalCost += trade.Quantity * trade.UnitPrice;
                        break;

                    case TradeSide.Sell:
                        if (trade.Quantity > totalQuantity)
                        {
                            throw new InvalidOperationException(
                                $"Cannot sell more than the current position for instrument {trade.InstrumentId}." +
                                $" Current position: {totalQuantity}, attempted to sell: {trade.Quantity}");
                        }

                        decimal currentAverageCost = totalCost / totalQuantity;
                        realizedProfitLoss += (trade.UnitPrice - currentAverageCost) * trade.Quantity;
                        totalQuantity -= trade.Quantity;
                        totalCost -= trade.Quantity * currentAverageCost;
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Unsupported trade side: {trade.Side}");
                }
            }

            decimal averageCost = totalQuantity == 0m ? 0m : totalCost / totalQuantity;

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