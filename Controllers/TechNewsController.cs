using LibraryApp.Feature.TechNews.DTOs;
using LibraryApp.Feature.TechNews.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

[ApiController]
[Route("api/tech-news")]
public class TechNewsController : ControllerBase
{
    private readonly ITechNewsAggregator _aggregator;
    private readonly ILogger<TechNewsController> _logger;

    public TechNewsController(ITechNewsAggregator aggregator, ILogger<TechNewsController> logger)
    {
        _aggregator = aggregator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<TechNewsPagedResponse>> GetTechNews([FromQuery] TechNewsQueryParameters query)
    {
        try
        {
            var response = await _aggregator.GetTechUpdatesAsync(query);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve tech news updates");
            return StatusCode(500, new { message = "Error loading latest tech news." });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> RefreshTechNews()
    {
        try
        {
            var updated = await _aggregator.RefreshAllUpdatesAsync();
            return Ok(new { message = "Tech news cache refreshed successfully.", count = updated.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh tech news cache");
            return StatusCode(500, new { message = "Error refreshing tech news." });
        }
    }
}
