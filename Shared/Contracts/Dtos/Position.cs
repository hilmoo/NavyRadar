namespace SharedModels.Contracts.Dtos;

public class Position
{
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required DateTime Timestamp { get; set; }
}
