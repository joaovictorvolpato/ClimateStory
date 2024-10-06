using System;
using System.Net.Http;
using System.Text;
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
        ConfigureHttpClient();
    }

    public void ConfigureHttpClient()
    {
        _httpClient.DefaultRequestHeaders
            .Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("curl/7.68.0");
    }
    public async Task<string> CheckSTACApiIsRunning()
    {
        try
        {
            var ApiUrl = "https://earth.gov/ghgcenter/api/stac/collections/odiac-ffco2-monthgrid-v2023";
            var response = await _httpClient.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            return jsonResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching API data: {ex.Message}");
            return default;
        }
    }

    public async Task<ResponseFromExternalApi> GetItensFromSTACApi()
    {
        try
        {
            var ApiUrl = $"{STAC_API_URL}/collections/{collection_name}/items?limit=300";
            var response = await _httpClient.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();
            
            var jsonObject = JObject.Parse(await response.Content.ReadAsStringAsync());

            var json = jsonObject["features"];
            
            return new ResponseFromExternalApi(){json = json, Num = (int)jsonObject["context"]["returned"]};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Dictionary<string, object>>> GetStatsForRegionAsync(double lat, double longi)
    {
        var toprightlati = ConvertLatitude( lat + 0.4);
        var toprightlong = ConvertLongitude(longi - 0.4);
        
        var topleftlati = ConvertLatitude(lat + 0.4);
        var topleftlong = ConvertLongitude(longi + 0.4);
        
        var bottomrightlati = ConvertLatitude(lat - 0.4);
        var bottomrightlong = ConvertLongitude(longi - 0.4);

        var bottomleftlati = ConvertLatitude(lat - 0.4);
        var bottomleftlong = ConvertLongitude(longi + 0.4);

        var AreaOfInterst = GeoJsonHelper.CreateAreaOfInterest(bottomrightlati, bottomrightlong, toprightlati,
            toprightlong, topleftlati, topleftlong, bottomleftlati, bottomleftlong);

        var items = await GetItensFromSTACApi();
        
        var JsonItems = items.json;

        var stats = await GenerateStatsForItemsAsync(JsonItems, AreaOfInterst);
        
        return stats;
    }
    
    public async Task<List<Dictionary<string, object>>> GenerateStatsForItemsAsync(JToken items, JObject geojson)
    {
        var results = new List<Dictionary<string, object>>();
        
        if (items is JArray)
        {
            foreach (var item in items)
            {
                if (item is JObject jsonItem)
                {
                    var stats = await GenerateStatsAsync(jsonItem, geojson);
                    if (stats != null)
                    {
                        results.Add(stats);
                    }
                }
            }
        }

        return results;
    }
    
    public async Task<Dictionary<string, object>> GenerateStatsAsync(JObject item, JObject geojson1)
    {
        try
        {
            // Extract the URL for the POST request from the item and asset name
            string assetUrl = item["assets"][asset_name]["href"].ToString();

            // Create the request URI for the statistics endpoint
            string requestUrl = $"{RASTER_API_URL}/cog/statistics";

            // Prepare the POST request with the parameters and GeoJSON data
            var content = new StringContent(geojson1.ToString(), Encoding.UTF8, "application/json");
            
            Console.WriteLine(content.ReadAsStringAsync());
            
            var requestUri = $"{requestUrl}?url={assetUrl}";
            var response = await _httpClient.PostAsync(requestUri, content);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            // Parse the response into a JObject
            var jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());

            // Extract properties from the response and the item
            var resultProperties = jsonResponse["properties"].ToObject<Dictionary<string, object>>();
            resultProperties["end_datetime"] = item["properties"]["end_datetime"].ToString();

            // Return the combined result as a dictionary
            return resultProperties;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating statistics: {ex.Message}");
            return null;
        }
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
    public JToken? json { get; set; }
    public int? Num { get; set; }
}

public class GeoJsonHelper
{
    static public JObject CreateAreaOfInterest(
        int bottomrightlati, int bottomrightlong,
        int toprightlati, int toprightlong,
        int topleftlati, int topleftlong,
        int bottomleftlati, int bottomleftlong)
    {
        
        Console.WriteLine($"Bottom Right Latitude: {bottomrightlati}, Bottom Right Longitude: {bottomrightlong}");
        Console.WriteLine($"Top Right Latitude: {toprightlati}, Top Right Longitude: {toprightlong}");
        Console.WriteLine($"Top Left Latitude: {topleftlati}, Top Left Longitude: {topleftlong}");
        Console.WriteLine($"Bottom Left Latitude: {bottomleftlati}, Bottom Left Longitude: {bottomleftlong}");

        string array = $@"[ 
            [[{bottomrightlong},{bottomrightlati}], 
             [{toprightlong}, {toprightlati}], 
             [{topleftlong},{topleftlati}], 
             [{bottomleftlong},{bottomleftlati}], 
             [{bottomrightlong},{bottomrightlati}]] 
        ]";
        
        JArray jsonArray = JArray.Parse(array);
            
        Console.WriteLine(jsonArray);
        
        
        var json = new JObject(
            new JProperty("type", "Feature"),
            new JProperty("properties", new JObject()),
            new JProperty("geometry", 
                new JObject(
                    new JProperty("coordinates", 
                        new JArray(jsonArray)
                    ),
                    new JProperty("type", "Polygon")
                )
            )
        );
        
        return json;
    }
}
