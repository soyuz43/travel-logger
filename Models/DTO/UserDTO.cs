namespace TravelLogger.Models.DTO;

public class UserDTO
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Description { get; set; }
    public string PhotoUrl { get; set; }
    public LogDTO Log { get; set; }
    public CitiesDTO? City { get; set; }
    public List<UpVoteDTO>? UpVote { get; set; }

}