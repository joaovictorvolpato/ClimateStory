using ClimateStory.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClimateStory.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FecthDataController : ControllerBase
{
    private readonly IFecthData _statisticsService;

    public FecthDataController(IFecthData statisticsService)
    {
        _statisticsService = statisticsService;
    }

    // GET: api/statistics?latitude=35.6895&longitude=139.6917
    [HttpGet("get-statistics")]
    public async Task<IActionResult> GetStatistics([FromQuery] double latitude, [FromQuery] double longitude)
    {
        if (latitude == 0 || longitude == 0)
        {
            return BadRequest("Invalid coordinates.");
        }

        // Call the service to fetch data based on coordinates
        var statisticsData = await _statisticsService.GetStatsForRegionAsync(latitude, longitude);

        // Return the statistics data (as JSON)
        return Ok(statisticsData);
    }
}