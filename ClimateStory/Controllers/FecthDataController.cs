using ClimateStory.Models;
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
    public async Task<ActionResult<List<YearlyStatistics>>> GetStatistics([FromQuery] double latitude, [FromQuery] double longitude)
    {
        if (latitude == 0 || longitude == 0)
        {
            return BadRequest("Invalid coordinates.");
        }

        // Call the service to fetch data based on coordinates
        var statisticsData = await _statisticsService.GetStatsForRegionAsync(latitude, longitude);

        var response = YearlyStatistics.ConvertToYearlyStatisticsAll(statisticsData);
        // Return the statistics data (as JSON)
        return Ok(response);
    }
    
    [HttpGet("get-itens")]
    public async Task<ActionResult> GetItens()
    {
        var items = await _statisticsService.GetItensFromSTACApi();
        // Return the statistics data (as JSON)
        return Ok(items.json.ToString());
    }
    
    [HttpGet("check-api")]
    public async Task<ActionResult> CheckAPIisUP()
    {
        var isUp = await _statisticsService.CheckSTACApiIsRunning();

        // Return the statistics data (as JSON)
        return Ok(isUp.ToString());
    }
}