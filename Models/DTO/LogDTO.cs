namespace TravelLogger.Models.DTO;

public class LogDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Comment { get; set; }
    public int UserId { get; set; }
    public UserDTO User { get; set; }
    public int CityId { get; set; }
    public CitiesDTO City { get; set; }

    public DateTime CreatedAt { get; set; }
}