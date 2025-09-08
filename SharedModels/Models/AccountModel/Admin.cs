namespace SharedModels.Models.AccountModel;

using SharedModels.Models.ShipModel;

public class Admin : Account
{
    public Admin() : base() { }
    public Admin(Account account) : base(account) { }

    public static void AddShip(Ship ship, ShipRadarService radarService)
    {
        radarService.AddShip(ship);
    }

    public static void RemoveShip(string shipId, ShipRadarService radarService)
    {
        radarService.RemoveShip(shipId);
    }

    public static void UpdateShipData(Ship updatedShip, ShipRadarService radarService)
    {
        radarService.UpdateShipData(updatedShip);
    }

    public static void AssignCaptainToShip(string captainId, string shipId, CaptainService captainService)
    {
        captainService.AssignShipToCaptain(captainId, shipId);
    }
}