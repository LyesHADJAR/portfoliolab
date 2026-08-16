using PortfolioLab.Domain.Trades;
using PortfolioLab.Domain.Positions;

namespace PortfolioLab.Domain.Tests
{
    public class PositionCalculatorTests
    {
        // Arrange, Act, Assert pattern for unit testing

        [Fact]
        public void CalculatePositions_SingleBuy_ReturnsExpectedPosition()
        {
            Trade trade = new Trade
            {
                TradeId = Guid.NewGuid(),
                InstrumentId = "AAPL",
                Side = TradeSide.Buy,
                Quantity = 100m,
                UnitPrice = 150m,
                ExecutedAt = DateTimeOffset.UtcNow
            };

            PositionCalculator calculator = new PositionCalculator();

            IReadOnlyCollection<Position> positions = calculator.CalculatePositions(new List<Trade> { trade });

            Position position = Assert.Single(positions);

            Assert.Equal("AAPL", position.InstrumentId);
            Assert.Equal(100m, position.Quantity);
            Assert.Equal(150m, position.AverageCost);
            Assert.Equal(0m, position.RealizedProfitLoss);

        }
    }
}
