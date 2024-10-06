using ClimateStory.Models;
using Newtonsoft.Json.Linq;

namespace ClimateStory.Services;

public interface IFecthData
{
    Task<List<Dictionary<string, object>>> GetStatsForRegionAsync(double lat, double longi);
    Task<ResponseFromExternalApi> GetItensFromSTACApi();
    Task<string> CheckSTACApiIsRunning();
}