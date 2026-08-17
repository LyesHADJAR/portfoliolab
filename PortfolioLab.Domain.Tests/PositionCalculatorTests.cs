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

        [Fact]
        public void CalculatePositions_MultipleBuysSameInstrument_ReturnsWeightedAveragePosition()
        {
            Trade firstTrade = new Trade
            {
                TradeId = Guid.NewGuid(),
                InstrumentId = "AAPL",
                Side = TradeSide.Buy,
                Quantity = 100m,
                UnitPrice = 150m,
                ExecutedAt = DateTimeOffset.UtcNow
            };

            Trade secondTrade = new Trade
            {
                TradeId = Guid.NewGuid(),
                InstrumentId = "AAPL",
                Side = TradeSide.Buy,
                Quantity = 50m,
                UnitPrice = 180m,
                ExecutedAt = DateTimeOffset.UtcNow
            };

            PositionCalculator calculator = new PositionCalculator();

            IReadOnlyCollection<Position> positions = calculator.CalculatePositions(new List<Trade> { firstTrade, secondTrade });

            Position position = Assert.Single(positions);

            Assert.Equal("AAPL", position.InstrumentId);
            Assert.Equal(150m, position.Quantity);
            Assert.Equal(160m, position.AverageCost);
            Assert.Equal(0m, position.RealizedProfitLoss);
        }

        [Fact]
        public void CalculatePositions_MultipleBuysDifferentInstruments_ReturnsSeparatePositions()
        {
            Trade firstTrade = new Trade
            {
                TradeId = Guid.NewGuid(),
                InstrumentId = "AAPL",
                Side = TradeSide.Buy,
                Quantity = 100m,
                UnitPrice = 150m,
                ExecutedAt = DateTimeOffset.UtcNow
            };

            Trade secondTrade = new Trade
            {
                TradeId = Guid.NewGuid(),
                InstrumentId = "MSFT",
                Side = TradeSide.Buy,
                Quantity = 50m,
                UnitPrice = 400m,
                ExecutedAt = DateTimeOffset.UtcNow
            };

            PositionCalculator calculator = new PositionCalculator();

            IReadOnlyCollection<Position> positions = calculator.CalculatePositions(new List<Trade> { firstTrade, secondTrade });

            Assert.Equal(2, positions.Count);

            Position aaplPosition = positions.Single(position => position.InstrumentId == "AAPL");
            Position msftPosition = positions.Single(position => position.InstrumentId == "MSFT");
            
            Assert.Equal(100m, aaplPosition.Quantity);
            Assert.Equal(150m, aaplPosition.AverageCost);
            Assert.Equal(0m, aaplPosition.RealizedProfitLoss);

            Assert.Equal(50m, msftPosition.Quantity);
            Assert.Equal(400m, msftPosition.AverageCost);
            Assert.Equal(0m, msftPosition.RealizedProfitLoss);
        }

        [Fact]
        public void CalculatePositions_BuyThenSell_ReturnsRemainingPositionAndRealizedProfitLoss()
        {
            Trade buy = new Trade
            {
                TradeId = Guid.NewGuid(),
                InstrumentId = "AAPL",
                Side = TradeSide.Buy,
                Quantity = 100m,
                UnitPrice = 150m,
                ExecutedAt = DateTimeOffset.UtcNow
            };

            Trade sell = new Trade
            {
                TradeId = Guid.NewGuid(),
                InstrumentId = "AAPL",
                Side = TradeSide.Sell,
                Quantity = 40m,
                UnitPrice = 180m,
                ExecutedAt = DateTimeOffset.UtcNow
            };

            PositionCalculator calculator = new PositionCalculator();

            IReadOnlyCollection<Position> positions = calculator.CalculatePositions(new List<Trade> { buy, sell });

            Position position = Assert.Single(positions);
            
            Assert.Equal("AAPL", position.InstrumentId);
            Assert.Equal(60m, position.Quantity);
            Assert.Equal(150m, position.AverageCost);
            Assert.Equal(1200m, position.RealizedProfitLoss);
        }

        [Fact]
        public void CalculatePositions_MultipleBuysThenSell_ReturnsWeightedAverageAndRealizedProfitLoss()
        {
            Trade firstBuy = new Trade
            {
                TradeId = Guid.NewGuid(),
                InstrumentId = "AAPL",
                Side = TradeSide.Buy,
                Quantity = 100m,
                UnitPrice = 150m,
                ExecutedAt = DateTimeOffset.UtcNow
            };

            Trade secondBuy = new Trade
            {
                TradeId = Guid.NewGuid(),
                InstrumentId = "AAPL",
                Side = TradeSide.Buy,
                Quantity = 50m,
                UnitPrice = 180m,
                ExecutedAt = DateTimeOffset.UtcNow
            };

            Trade sell = new Trade
            {
                TradeId = Guid.NewGuid(),
                InstrumentId = "AAPL",
                Side = TradeSide.Sell,
                Quantity = 30m,
                UnitPrice = 200m,
                ExecutedAt = DateTimeOffset.UtcNow
            };

            PositionCalculator calculator = new PositionCalculator();

            IReadOnlyCollection<Position> positions = calculator.CalculatePositions(new List<Trade> { firstBuy, secondBuy, sell });

            Position position = Assert.Single(positions);

            Assert.Equal("AAPL", position.InstrumentId);
            Assert.Equal(120m, position.Quantity);
            Assert.Equal(160m, position.AverageCost);
            Assert.Equal(1200m, position.RealizedProfitLoss);
        }
    }
}
