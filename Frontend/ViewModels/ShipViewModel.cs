using Frontend.Service.Api;
using SharedModels.Contracts.Dtos;

namespace Frontend.ViewModels;

public class ShipViewModel(IShipApiService shipService) : BaseViewModel
{
    private Task<Ship?>? _ship;

    public Task<Ship?>? Ship
    {
        get => _ship;
        private set
        {
            _ship = value;
            OnPropertyChanged();
        }
    }

    public void LoadShip(string shipId)
    {
        Ship = shipService.GetShipById(shipId);
    }
}