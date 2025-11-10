using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Input;
using NavyRadar.Frontend.Util;

namespace NavyRadar.Frontend.ViewModels;

public class HomeVm : ViewModelBase
{
    public Shared.Spec.Port? SelectedPort
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Shared.Spec.Ship? SelectedShip
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Shared.Spec.Sail? SelectedSail
    {
        get;
        set => SetProperty(ref field, value);
    }

    public string ShipNameFilter
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;

    public bool IsCargoVesselsChecked
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsTankersChecked
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsPassengerVesselsChecked
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsHighSpeedCraftChecked
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsTugsAndSpecialCraftChecked
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsFishingChecked
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsPleasureCraftChecked
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsNavigationAidsChecked
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsUnspecifiedShipsChecked
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ICommand ClearSelectionCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public event Action<string>? SendMessageToWebViewRequest;

    public HomeVm()
    {
        ClearSelectionCommand = new RelayCommand(ClearSelection);
        ApplyFilterCommand = new RelayCommand(ApplyFilter);
    }

    private void ClearSelection(object? obj)
    {
        SelectedPort = null;
        SelectedShip = null;
        SelectedSail = null;
    }

    private void ApplyFilter(object? obj)
    {
        var shipTypes = new JsonObject
        {
            ["cargoVessels"] = IsCargoVesselsChecked,
            ["tankers"] = IsTankersChecked,
            ["passengerVessels"] = IsPassengerVesselsChecked,
            ["highSpeedCraft"] = IsHighSpeedCraftChecked,
            ["tugsAndSpecialCraft"] = IsTugsAndSpecialCraftChecked,
            ["fishing"] = IsFishingChecked,
            ["pleasureCraft"] = IsPleasureCraftChecked,
            ["navigationAids"] = IsNavigationAidsChecked,
            ["unspecifiedShips"] = IsUnspecifiedShipsChecked
        };

        var message = new JsonObject
        {
            ["shipName"] = ShipNameFilter,
            ["types"] = shipTypes
        };

        SendMessageToWebViewRequest?.Invoke(message.ToJsonString());
    }

    public async void ProcessWebMessage(string jsonMessage)
    {
        try
        {
            if (string.IsNullOrEmpty(jsonMessage))
            {
                return;
            }

            try
            {
                var messageNode = JsonNode.Parse(jsonMessage);
                if (messageNode == null) return;
                var messageType = messageNode["type"]?.GetValue<string>();
                ClearSelection(null);

                switch (messageType)
                {
                    case "portSelected":
                        var portId = messageNode["id"]?.GetValue<int>();
                        if (portId == null) return;
                        SelectedPort = await ApiService.ApiClient.PortsGETAsync(portId.Value);
                        Debug.WriteLine($"PORT SELECTED -> ID: {SelectedPort.Id}");
                        break;

                    case "shipSelected":
                        var shipId = messageNode["shipId"]?.GetValue<int>();
                        var sailId = messageNode["sailId"]?.GetValue<int>();
                        if (shipId == null || sailId == null) return;
                        SelectedShip = await ApiService.ApiClient.ShipGETAsync(shipId.Value);
                        SelectedSail = await ApiService.ApiClient.SailsGETAsync(sailId.Value);
                        Debug.WriteLine($"SHIP SELECTED -> ShipID: {SelectedShip.Id}, SailID: {SelectedSail.Id}");
                        break;

                    default:
                        Debug.WriteLine($"Unknown JSON message received: {jsonMessage}");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to parse message: {ex.Message}", "Error");
            }
        }
        catch (Exception e)
        {
            MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error");
        }
    }
}