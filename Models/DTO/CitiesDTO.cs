namespace TravelLogger.Models.DTO;

public class CitiesDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<RecommendationDTO> Recommendations { get; set; }
    public List<LogDTO> Log { get; set; }
}