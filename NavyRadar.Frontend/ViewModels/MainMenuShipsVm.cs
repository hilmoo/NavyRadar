using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Frontend.Views.Dialog;
using NavyRadar.Shared.Entities;
using ShipEntity = NavyRadar.Shared.Entities.Ship;

namespace NavyRadar.Frontend.ViewModels;

public class MainMenuShipsVm : ViewModelBase
{
    public ObservableCollection<ShipEntity> Ships { get; } = [];

    [field: AllowNull, MaybeNull]
    public ShipEntity CurrentShip
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    }

    public bool IsAdmin
    {
        get;
        init
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public event Func<string, string, bool>? ConfirmationRequested;

    public ICommand UpdateShipCommand { get; }
    public ICommand NewShipCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand RemoveShipCommand { get; }

    public MainMenuShipsVm()
    {
        UpdateShipCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnUpdateShipAsync),
            () => !IsLoading);

        NewShipCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnNewShipAsync),
            () => !IsLoading);

        RefreshCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(LoadShipsAsync),
            () => !IsLoading);

        RemoveShipCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnRemoveShipAsync),
            () => !IsLoading);

        _ = ExecuteLoadingTask(LoadShipsAsync);
    }

    private async Task LoadShipsAsync()
    {
        Ships.Clear();

        var shipsFromServer = await ApiService.ApiClient.ShipAllAsync();
        foreach (var ship in shipsFromServer)
        {
            Ships.Add(ship.ToEntity());
        }

        CurrentShip = Ships.FirstOrDefault()!;
    }

    private async Task OnUpdateShipAsync()
    {
        var shipToEdit = new ShipEntity
        {
            Id = CurrentShip.Id,
            Name = CurrentShip.Name,
            ImoNumber = CurrentShip.ImoNumber,
            MmsiNumber = CurrentShip.MmsiNumber,
            Type = CurrentShip.Type,
            YearBuild = CurrentShip.YearBuild,
            LengthOverall = CurrentShip.LengthOverall,
            GrossTonnage = CurrentShip.GrossTonnage
        };

        var dialog = new MainMenuShipDialog(shipToEdit);

        if (dialog.ShowDialog() == true)
        {
            if (dialog.Ship != null)
            {
                var updatedShip =
                    await ApiService.ApiClient.ShipPUTAsync(dialog.Ship.Id, dialog.Ship.ToDto());

                var updatedShipEntity = updatedShip.ToEntity();
                var originalShipInList = Ships.FirstOrDefault(s => s.Id == updatedShipEntity.Id);

                if (originalShipInList != null)
                {
                    var index = Ships.IndexOf(originalShipInList);

                    if (index != -1)
                    {
                        Ships[index] = updatedShipEntity;
                        CurrentShip = updatedShipEntity;
                    }
                }
            }
        }
    }

    private async Task OnNewShipAsync()
    {
        var dialog = new MainMenuShipDialog(new ShipEntity
        {
            Name = "",
            ImoNumber = "",
            MmsiNumber = "",
            Type = ShipType.UnspecifiedShips,
            YearBuild = 0,
            LengthOverall = 0,
            GrossTonnage = 0
        });
        if (dialog.ShowDialog() == true)
        {
            if (dialog.Ship != null)
            {
                var newShip = await ApiService.ApiClient.ShipPOSTAsync(dialog.Ship.ToDto());
                Ships.Add(newShip.ToEntity());
                CurrentShip = newShip.ToEntity();
            }
        }
    }

    private async Task OnRemoveShipAsync()
    {
        var confirmed = ConfirmationRequested?.Invoke(
            "Confirm Removal",
            $"Are you sure you want to remove {CurrentShip.Name}?") ?? false;

        if (!confirmed) return;

        var shipToRemove = CurrentShip;
        var index = Ships.IndexOf(shipToRemove);

        await ApiService.ApiClient.ShipDELETEAsync(shipToRemove.Id);

        Ships.Remove(shipToRemove);

        CurrentShip = (Ships.Any() ? Ships[Math.Max(0, index - 1)] : null)!;
    }
}