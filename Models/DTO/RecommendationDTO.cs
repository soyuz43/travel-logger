namespace TravelLogger.Models.DTO;

public class RecommendationDTO
{
    public int Id { get; set; }
    public string Place { get; set; }
    public int CitiesId { get; set; }
    public int UpVoteId { get; set; }
    public List<UpVoteDTO>? UpVote { get; set; }
    public CitiesDTO? Cities { get; set; }
}