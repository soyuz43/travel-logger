namespace TravelLogger.Models;

public class Log
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Comment { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int CityId { get; set; }
    public Cities? City { get; set; }
    public DateTime CreatedAt { get; set; }
    // public Recommendation Recommendation { get; set; }
}