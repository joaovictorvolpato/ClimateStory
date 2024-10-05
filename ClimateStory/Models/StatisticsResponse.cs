namespace ClimateStory.Models;

public class MonthlyCO2Statistics
{
    public int Year { get; set; }
    public int Month { get; set; }
    public double MeanCO2 { get; set; }
    public double MaxCO2 { get; set; }
    public double MinCO2 { get; set; }
}

public class StatisticsResponse
{
    public List<MonthlyCO2Statistics> MonthlyStatistics { get; set; } = new List<MonthlyCO2Statistics>();
    
    public StatisticsResponse()
    {
    }

    public StatisticsResponse(List<MonthlyCO2Statistics> monthlyStatistics)
    {
        MonthlyStatistics = monthlyStatistics;
    }
}