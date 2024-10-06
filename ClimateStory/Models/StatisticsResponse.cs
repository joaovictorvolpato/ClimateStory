using Newtonsoft.Json.Linq;

namespace ClimateStory.Models;

public class YearlyStatistics
{
    public int Year { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double Mean { get; set; }
    public double TotalSum { get; set; } // Sums the sum field for all months

    public static List<YearlyStatistics> ConvertToYearlyStatistics(List<Dictionary<string, object>> dataList)
    {
        var yearlyStatsList = new List<YearlyStatistics>();

        foreach (var dictionary in dataList)
        {
            // Extract the end_datetime field to get the year
            if (dictionary.ContainsKey("end_datetime"))
            {
                var date = dictionary["end_datetime"];
                int year = DateTime.Parse(date.ToString()).Year;

                // Extract the statistics field (it is expected to be a nested object)
                if (dictionary.ContainsKey("statistics") && dictionary["statistics"] is JObject statistics)
                {
                    var b1 = statistics["b1"];
                    if (b1 != null)
                    {
                        double min = b1.Value<double>("min");
                        double max = b1.Value<double>("max");
                        double mean = b1.Value<double>("mean");
                        double sum = b1.Value<double>("sum");

                        // Create the YearlyStatistics object and add it to the list
                        var yearlyStats = new YearlyStatistics
                        {
                            Year = year,
                            Min = min,
                            Max = max,
                            Mean = mean,
                            TotalSum = sum
                        };

                        yearlyStatsList.Add(yearlyStats);
                    }
                }
            }
        }

        return yearlyStatsList;
    }

    public static List<YearlyStatistics> ConvertToYearlyStatisticsAll(List<Dictionary<string, object>> dataList)
    {
        var yearlyStatsDictionary = new Dictionary<int, YearlyStatistics>();

        foreach (var dictionary in dataList)
        {
            // Extract the end_datetime field to get the year
            if (dictionary.ContainsKey("end_datetime"))
            {
                var date = dictionary["end_datetime"];
                int year = DateTime.Parse(date.ToString()).Year;

                // Extract the statistics field (it is expected to be a nested object)
                if (dictionary.ContainsKey("statistics") && dictionary["statistics"] is JObject statistics)
                {
                    var b1 = statistics["b1"];
                    if (b1 != null)
                    {
                        double min = b1.Value<double>("min");
                        double max = b1.Value<double>("max");
                        double mean = b1.Value<double>("mean");
                        double sum = b1.Value<double>("sum");

                        // Check if the year already exists in the dictionary
                        if (!yearlyStatsDictionary.ContainsKey(year))
                        {
                            // If not, create a new YearlyStatistics entry
                            yearlyStatsDictionary[year] = new YearlyStatistics
                            {
                                Year = year,
                                Min = min,
                                Max = max,
                                Mean = mean,
                                TotalSum = sum // Start with the sum for the first month
                            };
                        }
                        else
                        {
                            // If the year already exists, update the existing entry
                            var currentStats = yearlyStatsDictionary[year];

                            currentStats.Min = Math.Min(currentStats.Min, min);
                            currentStats.Max = Math.Max(currentStats.Max, max);
                            currentStats.Mean = (currentStats.Mean + mean) / 2; // Averaging the means
                            currentStats.TotalSum += sum; // Accumulate the sum for the year
                        }
                    }
                }
            }
        }

        // Convert the dictionary to a list and return it
        return yearlyStatsDictionary.Values.ToList();
    }
}

