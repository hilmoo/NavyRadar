namespace SharedModels.Models.AccountModel;

using SharedModels.Models.ShipModel;

public class User : Account
{
    public User() : base() { }
    public User(Account account) : base(account) { }

    public static List<Ship> ViewAllShips(ShipRadarService radarService)
    {
        return radarService.GetAllShips();
    }

    public static Ship? ViewShipById(string shipId, ShipRadarService radarService)
    {
        return radarService.GetShipById(shipId);
    }
}