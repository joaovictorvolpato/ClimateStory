using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using ClimateStory.Models;
using Newtonsoft.Json.Linq;

namespace ClimateStory.Services;

public class FecthData : IFecthData
{
    static string STAC_API_URL = "https://earth.gov/ghgcenter/api/stac";
    static string RASTER_API_URL = "https://earth.gov/ghgcenter/api/raster";
    static string collection_name = "odiac-ffco2-monthgrid-v2023";
    private static string items_url = $"{STAC_API_URL}/collections/{collection_name}/items";
    private static string asset_name = "co2-emissions";
    
    private readonly HttpClient _httpClient;
    // Inject HttpClient via dependency injection
    public FecthData(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CheckSTACApiIsRunning()
    {
        try
        {
            var ApiUrl = $"{STAC_API_URL}/collections/{collection_name}";
            var response = await _httpClient.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching API data: {ex.Message}");
            return default;
        }
    }

    private async Task<ResponseFromExternalApi> GetItensFromSTACApi()
    {
        try
        {
            var ApiUrl = $"{STAC_API_URL}/collections/{collection_name}/items?limit=300";
            var response = await _httpClient.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();
            
            var jsonObject = JObject.Parse(await response.Content.ReadAsStringAsync());

            // Access the "features" field from the JSON response
            var features = jsonObject["features"];

            int size = 0;
            foreach (var feat in features)
            {
                size++;
            }
            return new ResponseFromExternalApi(){JObject = jsonObject, Num = size};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<StatisticsResponse> GetStatsForRegionAsync(double lat, double longi)
    {
        var toprightlati = ConvertLatitude( lat + 0.4);
        var toprightlong = ConvertLongitude(longi - 0.4);
        
        var topleftlati = ConvertLatitude(lat + 0.4);
        var topleftlong = ConvertLongitude(longi + 0.4);
        
        var bottomrightlati = ConvertLatitude(lat - 0.4);
        var bottomrightlong = ConvertLongitude(longi - 0.4);

        var bottomleftlati = ConvertLatitude(lat - 0.4);
        var bottomleftlong = ConvertLongitude(longi + 0.4);
        
        
        var AreaOfInterst = $@" {{""type"": ""Feature"",
                            ""properties"": {{}},
                            ""geometry"": {{ 
                                ""coordinates"": [
                                    [[{bottomrightlati}, {bottomrightlong}], 
                                    [{toprightlati}, {toprightlong}], 
                                    [{topleftlati},{topleftlong}], 
                                    [{bottomleftlati},{bottomleftlong}], 
                                    [{bottomrightlati}, {bottomrightlong}] ]
                                    ],
                                ""type"": ""Polygon"",
                            }},
                            }}";

        return null;
    }

    private int ConvertLatitude(double lati)
    {
        return (int)Math.Floor(30 + (lati - 13));
    }

    private int ConvertLongitude(double longi)
    {
        return (int)Math.Floor(-105 + (23 + longi) / 2);
    }
}

public class ResponseFromExternalApi()
{
    public JObject? JObject { get; set; }
    public int? Num { get; set; }
}