using ClimateStory.Models;

namespace ClimateStory.Services;

public interface IFecthData
{
    Task<StatisticsResponse> GetStatsForRegionAsync(double lat, double longi);

}