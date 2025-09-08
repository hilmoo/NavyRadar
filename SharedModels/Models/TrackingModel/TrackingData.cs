namespace SharedModels.Models.TrackingModel;

using SharedModels.Models.ShipModel;

public class TrackingData
{
    public required string ShipId { get; set; }
    public required List<Position> PositionHistory { get; set; } = [];
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required double TotalDistance { get; set; }
    public required double AverageSpeed { get; set; }
    public required double MaxSpeed { get; set; }
}
