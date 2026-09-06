using Microsoft.AspNetCore.Mvc;
using PortfolioLab.Api.Dtos;
using PortfolioLab.Domain.Positions;
using PortfolioLab.Domain.Trades;
using PortfolioLab.Infrastructure.Persistence;
using PortfolioLab.Api.Mappers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    // inject PositionCalculator and TradeRepository via constructor (Dependency Injection)
    private readonly PositionCalculator _positionCalculator;
    private readonly TradeRepository _tradeRepository;
    public PositionsController(PositionCalculator positionCalculator, TradeRepository tradeRepository)
    {
        _positionCalculator = positionCalculator;
        _tradeRepository = tradeRepository;
    }

    [HttpPost("calculate")]

    public IActionResult Calculate(List<TradeRequest> tradeRequests)
    {
        // map TradeRequest -> Trade (helper for TradeId)
        // try/catch around PositionCalculator call
        // map Position -> PositionResponse
        // return Ok(...) or BadRequest(new ProblemDetails { ... })


        if (tradeRequests == null || !tradeRequests.Any())
        {
            return BadRequest(new ProblemDetails { Title = "Invalid trade data", Detail = "No valid trades provided." });
        }

        IReadOnlyCollection<Trade> trades = tradeRequests.Select(TradeMapper.ToTrade).ToList();

        try
        {
            IReadOnlyCollection<Position> position = _positionCalculator.CalculatePositions(trades);
            IReadOnlyCollection<PositionResponse> positionResponses = PositionResponsesMapper.MapToPositionResponses(position);
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
    }

    [HttpGet]
    public async Task<IActionResult> CalculateAll()
    {
        // load all stored trades from TradeRepository
        // try/catch around PositionCalculator call
        // return 200 and catch 400 with ProblemDetails if exception occurs

        IReadOnlyCollection<Trade> trades = await _tradeRepository.GetAllAsync();
        try
        {
            IReadOnlyCollection<Position> position = _positionCalculator.CalculatePositions(trades);
            IReadOnlyCollection<PositionResponse> positionResponses = PositionResponsesMapper.MapToPositionResponses(position);
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
    }

}