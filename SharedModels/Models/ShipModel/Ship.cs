namespace SharedModels.Models.ShipModel;

public abstract class Ship
{
    public required string Id { get; set; }
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

    protected Ship() { }

    protected Ship(Ship ship)
    {
        Id = ship.Id;
        Name = ship.Name;
        Type = ship.Type;
        YearBuild = ship.YearBuild;
        LengthOverall = ship.LengthOverall;
        GrossTonnage = ship.GrossTonnage;
        CurrentPosition = ship.CurrentPosition;
        Speed = ship.Speed;
        Heading = ship.Heading;
        Status = ship.Status;
        LastUpdate = ship.LastUpdate;
        Destination = ship.Destination;
        EstimatedArrival = ship.EstimatedArrival;
    }
}