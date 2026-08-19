using Microsoft.AspNetCore.Mvc;
using PortfolioLab.Api.Dtos;
using PortfolioLab.Domain.Positions;
using PortfolioLab.Domain.Trades;

[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    [HttpPost("calculate")]
    public IActionResult Calculate(List<TradeRequest> tradeRequests)
    {
        // map TradeRequest -> Trade (helper for TradeId)
        // try/catch around PositionCalculator call
        // map Position -> PositionResponse
        // return Ok(...) or BadRequest(new ProblemDetails { ... })

        IReadOnlyCollection<Trade> trades = tradeRequests.Select(MapToTrade).ToList();
        
        try
        {
            PositionCalculator positionCalculator = new();
            IReadOnlyCollection<Position> position = positionCalculator.CalculatePositions(trades);

            IReadOnlyCollection<PositionResponse> positionResponses = position.Select(p => new PositionResponse
            {
                InstrumentId = p.InstrumentId,
                Quantity = p.Quantity,
                AverageCost = p.AverageCost,
                RealizedProfitLoss = p.RealizedProfitLoss
            }).ToList();
            return Ok(positionResponses);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid trade data", Detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        { 
            return BadRequest(new ProblemDetails { Title = "Invalid trade sequence", Detail = ex.Message }); 
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails { Title = "An error occurred", Detail = ex.Message });
        }

    }

    private Trade MapToTrade(TradeRequest tradeRequest)
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
}