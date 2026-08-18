using PortfolioLab.Domain.Trades;
using PortfolioLab.Domain.Positions;

namespace PortfolioLab.Domain.Tests
{
    public class PositionCalculatorTests
    {
        Trade CreateTrade(TradeSide side, decimal quantity, decimal unitPrice, string instrumentId = "AAPL", DateTimeOffset? executedAt = null, Guid? tradeId = null)
        {
            return new Trade
            {
                InstrumentId = instrumentId,
                Side = side,
                Quantity = quantity,
                UnitPrice = unitPrice,
                ExecutedAt = executedAt ?? DateTimeOffset.UtcNow,
                TradeId = tradeId ?? Guid.NewGuid()
            };
        }

        // Arrange, Act, Assert pattern for unit testing

        [Fact]
        public void CalculatePositions_SingleBuy_ReturnsExpectedPosition()
        {
            Trade trade = CreateTrade(TradeSide.Buy, 100m, 150m);

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
            Trade firstTrade = CreateTrade(TradeSide.Buy, 100m, 150m);

            Trade secondTrade = CreateTrade(TradeSide.Buy, 50m, 180m);

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
            Trade firstTrade = CreateTrade(TradeSide.Buy, 100m, 150m, instrumentId: "AAPL");

            Trade secondTrade = CreateTrade(TradeSide.Buy, 50m, 400m, instrumentId: "MSFT");

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
            Trade buy = CreateTrade(TradeSide.Buy, 100m, 150m);

            Trade sell = CreateTrade(TradeSide.Sell, 40m, 180m);

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
            Trade firstBuy = CreateTrade(TradeSide.Buy, 100m, 150m);
            
            Trade secondBuy = CreateTrade(TradeSide.Buy, 50m, 180m);

            Trade sell = CreateTrade(TradeSide.Sell, 30m, 200m);

            PositionCalculator calculator = new PositionCalculator();

            IReadOnlyCollection<Position> positions = calculator.CalculatePositions(new List<Trade> { firstBuy, secondBuy, sell });

            Position position = Assert.Single(positions);

            Assert.Equal("AAPL", position.InstrumentId);
            Assert.Equal(120m, position.Quantity);
            Assert.Equal(160m, position.AverageCost);
            Assert.Equal(1200m, position.RealizedProfitLoss);
        }

        [Fact]
        public void CalculatePositions_TradesProvidedOutOfOrder_ProcessesByExecutionTime()
        {
            Trade sell = CreateTrade(TradeSide.Sell, 30m, 200m, executedAt: new DateTimeOffset(2026, 8, 17, 11, 0, 0, TimeSpan.Zero));

            Trade buy = CreateTrade(TradeSide.Buy, 100m, 150m, executedAt: new DateTimeOffset(2026, 8, 17, 10, 0, 0, TimeSpan.Zero));

            PositionCalculator calculator = new PositionCalculator();

            IReadOnlyCollection<Position> positions = calculator.CalculatePositions(new List<Trade> { sell, buy });  // Set buy before sell but pass them to the list in the wrong order

            Position position = Assert.Single(positions);

            Assert.Equal("AAPL", position.InstrumentId);
            Assert.Equal(70m, position.Quantity);
            Assert.Equal(150, position.AverageCost);
            Assert.Equal(1500m, position.RealizedProfitLoss);
        }

        [Fact]
        public void CalculatePositions_FullSell_ReturnsClosedPositionWithRealizedProfitLoss()
        {
            Trade buy = CreateTrade(TradeSide.Buy, 100m, 150m);

            Trade sell = CreateTrade(TradeSide.Sell, 100m, 180m);

            PositionCalculator calculator = new PositionCalculator();
            IReadOnlyCollection<Position> positions = calculator.CalculatePositions(new List<Trade> { buy, sell });

            Position position = Assert.Single(positions);

            Assert.Equal("AAPL", position.InstrumentId);
            Assert.Equal(0m, position.Quantity);
            Assert.Equal(0m, position.AverageCost);
            Assert.Equal(3000m, position.RealizedProfitLoss);
        }

        [Fact]
        public void CalculatePositions_SellAtLoss_ReturnsNegativeRealizedProfitLoss()
        {
            Trade buy = CreateTrade(TradeSide.Buy, 100m, 150m);

            Trade sell = CreateTrade(TradeSide.Sell, 40m, 120m);

            PositionCalculator calculator = new PositionCalculator();
            IReadOnlyCollection<Position> positions = calculator.CalculatePositions(new List<Trade> { buy, sell });

            Position position = Assert.Single(positions);

            Assert.Equal("AAPL", position.InstrumentId);
            Assert.Equal(60m, position.Quantity);
            Assert.Equal(150m, position.AverageCost);
            Assert.Equal(-1200m, position.RealizedProfitLoss);
        }

        [Fact]
        public void CalculatePositions_SellExceedsOwnedQuantity_ThrowsInvalidOperationException()
        {
            Trade buy = CreateTrade(TradeSide.Buy, 100m, 150m);

            Trade sell = CreateTrade(TradeSide.Sell, 120m, 180m);

            PositionCalculator calculator = new PositionCalculator();

            Assert.Throws<InvalidOperationException>(() => calculator.CalculatePositions(new List<Trade> { buy, sell }));
        }

        [Fact]
        public void CalculatePositions_DuplicateTradeIds_ThrowsInvalidOperationException()
        {
            Trade firstTrade = CreateTrade(TradeSide.Buy, 100m, 150m);

            Trade secondTrade = CreateTrade(TradeSide.Buy, 100m, 150m, tradeId: firstTrade.TradeId); // Using the same TradeId to create a duplicate

            PositionCalculator calculator = new PositionCalculator();

            Assert.Throws<InvalidOperationException>(() => calculator.CalculatePositions(new List<Trade> { firstTrade, secondTrade }));
        }

        [Fact]
        public void CalculatePositions_ZeroQuantityTrade_ThrowsArgumentOutOfRangeException()
        {
            Trade trade = CreateTrade(TradeSide.Buy, 0m, 150m);

            PositionCalculator calculator = new PositionCalculator();

            Assert.Throws<ArgumentOutOfRangeException>(() => calculator.CalculatePositions(new List<Trade> { trade }));
        }

        [Fact]
        public void CalculatePositions_NegativeQuantityTrade_ThrowsArgumentOutOfRangeException()
        {
            Trade trade = CreateTrade(TradeSide.Buy, -50m, 100m);

            PositionCalculator calculator = new PositionCalculator();

            Assert.Throws<ArgumentOutOfRangeException>(() => calculator.CalculatePositions(new List<Trade> { trade }));
        }

        [Fact]
        public void CalculatePositions_ZeroUnitPriceTrade_ThrowsArgumentOutOfRangeException()
        {
            Trade trade = CreateTrade(TradeSide.Buy, 50m, 0m);

            PositionCalculator calculator = new PositionCalculator();
            Assert.Throws<ArgumentOutOfRangeException>(() => calculator.CalculatePositions(new List<Trade> { trade }));
        }

        [Fact]
        public void CalculatePositions_NegativeUnitPriceTrade_ThrowsArgumentOutOfRangeException()
        {
            Trade trade = CreateTrade(TradeSide.Buy, 50m, -100m);

            PositionCalculator calculator = new PositionCalculator();
            Assert.Throws<ArgumentOutOfRangeException>(() => calculator.CalculatePositions(new List<Trade> { trade }));
        }

        [Fact]
        public void CalculatePositions_UnsupportedTradeSide_ThrowsInvalidOperationException()
        {
            Trade trade = CreateTrade((TradeSide)999, 50m, 100m);

            PositionCalculator calculator = new PositionCalculator();
            Assert.Throws<InvalidOperationException>(() => calculator.CalculatePositions(new List<Trade> { trade }));
        }
    }
}
