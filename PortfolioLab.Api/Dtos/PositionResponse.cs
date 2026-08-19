namespace PortfolioLab.Api.Dtos
{
    public class PositionResponse
    {
        public required string InstrumentId { get; init; }
        public required decimal Quantity { get; init; }
        public required decimal AverageCost { get; init; }
        public required decimal RealizedProfitLoss { get; init; }
    }
}
