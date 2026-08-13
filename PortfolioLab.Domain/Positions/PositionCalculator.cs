using PortfolioLab.Domain.Trades;

namespace PortfolioLab.Domain.Positions;

public sealed class PositionCalculator
{
    public IReadOnlyCollection<Position> CalculatePositions(
        IEnumerable<Trade> trades)
    {
        throw new NotImplementedException();
    }
}