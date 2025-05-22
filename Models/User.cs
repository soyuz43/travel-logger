namespace TravelLogger.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Description { get; set; }
    public string PhotoUrl { get; set; }
    public Log? Log { get; set; }
    public Cities? City { get; set; }
    public List<UpVote>? UpVote { get; set; }
    
}