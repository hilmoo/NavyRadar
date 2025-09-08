namespace SharedModels.Models.ShipModel;

public abstract class ShipRadarService
{
    private readonly Dictionary<string, Ship> _trackedShips = new();

    public Ship? GetShipById(string shipId)
    {
        return _trackedShips.GetValueOrDefault(shipId);
    }

    public List<Ship> GetAllShips()
    {
        return _trackedShips.Values.ToList();
    }

    public void AddShip(Ship ship)
    {
        _trackedShips[ship.Id] = ship;
    }

    public void RemoveShip(string shipId)
    {
        _trackedShips.Remove(shipId);
    }

    public void UpdateShipPosition(string shipId, Position newPosition)
    {
        if (!_trackedShips.TryGetValue(shipId, out var ship)) return;
        ship.CurrentPosition = newPosition;
        ship.LastUpdate = DateTime.UtcNow;
    }

    public void UpdateShipData(Ship updatedShip)
    {
        if (!_trackedShips.ContainsKey(updatedShip.Id)) return;
        _trackedShips[updatedShip.Id] = updatedShip;
    }

    public void UpdateShipStatus(string shipId, ShipStatus status)
    {
        if (!_trackedShips.TryGetValue(shipId, out Ship? ship)) return;
        ship.Status = status;
        ship.LastUpdate = DateTime.UtcNow;
    }

    public void UpdateShipDestination(string shipId, string destination, DateTime eta)
    {
        if (!_trackedShips.TryGetValue(shipId, out var ship)) return;
        ship.Destination = destination;
        ship.EstimatedArrival = eta;
        ship.LastUpdate = DateTime.UtcNow;
    }
}
