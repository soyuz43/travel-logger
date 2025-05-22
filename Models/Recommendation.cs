namespace TravelLogger.Models;

public class Recommendation
{
    public int Id { get; set; }
    public string Place { get; set; }
    public int CitiesId { get; set; }
    public int UpVoteId { get; set; }
    public List<UpVote>? UpVote { get; set; }
    public Log Log { get; set; }

}