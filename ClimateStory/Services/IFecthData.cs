using ClimateStory.Models;
using Newtonsoft.Json.Linq;

namespace ClimateStory.Services;

public interface IFecthData
{
    Task<StatisticsResponse> GetStatsForRegionAsync(double lat, double longi);
    Task<ResponseFromExternalApi> GetItensFromSTACApi();
    Task<string> CheckSTACApiIsRunning();
}