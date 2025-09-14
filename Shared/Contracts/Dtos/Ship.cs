using SharedModels.Contracts.Enums;

namespace SharedModels.Contracts.Dtos;

public class Ship
{
    public required string? Id { get; set; }
    public required string Name { get; set; }
    public required ShipType Type { get; set; }
    public required int YearBuild { get; set; }
    public required int LengthOverall { get; set; }
    public required int GrossTonnage { get; set; }
    public required string Destination { get; set; }
    public required string Origin { get; set; }
    public required double Speed { get; set; }
    public required double Heading { get; set; }
    public required Position CurrentPosition { get; set; }
    public required ShipStatus Status { get; set; }
    public required DateTime LastUpdate { get; set; }
    public required DateTime EstimatedArrival { get; set; }
}