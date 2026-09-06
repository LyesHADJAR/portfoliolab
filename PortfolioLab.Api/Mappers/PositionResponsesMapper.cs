using PortfolioLab.Api.Dtos;
using PortfolioLab.Domain.Positions;

namespace PortfolioLab.Api.Mappers
{
    public static class PositionResponsesMapper
    {
        public static IReadOnlyCollection<PositionResponse> MapToPositionResponses(
    IReadOnlyCollection<Position> positions)
        {
            return positions.Select(p => new PositionResponse
            {
                InstrumentId = p.InstrumentId,
                Quantity = p.Quantity,
                AverageCost = p.AverageCost,
                RealizedProfitLoss = p.RealizedProfitLoss
            }).ToList();
        }
    }
}
