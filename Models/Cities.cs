namespace TravelLogger.Models;

public class Cities
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Recommendation> Recommendations { get; set; }
    public List<Log> Log { get; set; }
}