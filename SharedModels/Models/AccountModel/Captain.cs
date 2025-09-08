namespace SharedModels.Models.AccountModel;

using SharedModels.Models.ShipModel;
using SharedModels.Models.TrackingModel;

public class Captain : Account
{
    public List<string> AssignedShipIds { get; set; } = [];
    public string? CurrentShipId { get; set; }
    public Captain() : base() { }
    public Captain(Account account) : base(account) { }

    public void SendPositionUpdate(Position position, ShipRadarService radarService, ShipHistoryService historyService)
    {
        if (CurrentShipId != null)
        {
            radarService.UpdateShipPosition(CurrentShipId, position);
            historyService.AddPositionToHistory(CurrentShipId, position);
        }
    }

    public void UpdateShipStatus(ShipStatus status, ShipRadarService radarService)
    {
        if (CurrentShipId != null)
        {
            radarService.UpdateShipStatus(CurrentShipId, status);
        }
    }

    public void UpdateDestination(string destination, DateTime eta, ShipRadarService radarService)
    {
        if (CurrentShipId != null)
        {
            radarService.UpdateShipDestination(CurrentShipId, destination, eta);
        }
    }
}