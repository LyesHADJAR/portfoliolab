using PortfolioLab.Api.Dtos;
using PortfolioLab.Domain.Trades;

namespace PortfolioLab.Api.Mappers
{
    public static class TradeMapper
    {
        public static Trade ToTrade(TradeRequest tradeRequest)
        {
            return new Trade
            {
                TradeId = Guid.NewGuid(),
                InstrumentId = tradeRequest.InstrumentId,
                Side = tradeRequest.Side,
                Quantity = tradeRequest.Quantity,
                UnitPrice = tradeRequest.UnitPrice,
                ExecutedAt = tradeRequest.ExecutedAt
            };
        }

        public static TradeResponse ToTradeResponse(Trade trade)
        {
            return new TradeResponse
            {
                TradeId = trade.TradeId,
                InstrumentId = trade.InstrumentId,
                Side = trade.Side,
                Quantity = trade.Quantity,
                UnitPrice = trade.UnitPrice,
                ExecutedAt = trade.ExecutedAt
            };
        }
    }
}
