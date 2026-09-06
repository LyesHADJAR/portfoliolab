using Microsoft.AspNetCore.Mvc;
using PortfolioLab.Api.Dtos;
using PortfolioLab.Api.Mappers;
using PortfolioLab.Domain.Trades;
using PortfolioLab.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class TradesController : ControllerBase
{

    // inject TradeRepository via constructor (Dependency Injection)
    private readonly TradeRepository _tradeRepository;
    public TradesController(TradeRepository tradeRepository)
    {
        _tradeRepository = tradeRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTrade(TradeRequest tradeRequest)
    {
        // map traderequest to trade entity
        // map trade entity to trade response
        // return CreatedAtAction with trade response

        Trade trade = TradeMapper.ToTrade(tradeRequest);
        Trade storedTrade = await _tradeRepository.AddAsync(trade);
        TradeResponse tradeResponse = TradeMapper.ToTradeResponse(storedTrade);
        return CreatedAtAction(null, new { id = tradeResponse.TradeId }, tradeResponse);

    }
}
