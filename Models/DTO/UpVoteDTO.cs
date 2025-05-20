namespace TravelLogger.Models.DTO;

public class UpVoteDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RecommendationId { get; set; }
    public RecommendationDTO? Recommendation { get; set; }
}