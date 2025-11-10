using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Frontend.Views.Dialog;
using NavyRadar.Shared.Spec;

namespace NavyRadar.Frontend.ViewModels;

public class MainMenuShipsVm : ObservableObject
{
    public ObservableCollection<Ship> Ships { get; } = [];

    [field: AllowNull, MaybeNull]
    public Ship CurrentShip
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    }

    private bool IsLoading
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
    } = false;

    public ICommand UpdateShipCommand { get; }
    public ICommand NewShipCommand { get; }
    public ICommand RefreshCommand { get; }

    public MainMenuShipsVm()
    {
        UpdateShipCommand = new SimpleRelayCommand(async void () =>
            {
                try
                {
                    await OnUpdateShipAsync();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error");
                }
            },
            () => !IsLoading);
        NewShipCommand = new SimpleRelayCommand(async void () =>
        {
            try
            {
                await OnNewShipAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error");
            }
        }, () => !IsLoading);
        RefreshCommand = new SimpleRelayCommand(async void () =>
        {
            try
            {
                await LoadShipsAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error");
            }
        }, () => !IsLoading);

        _ = LoadShipsAsync();
    }

    private async Task LoadShipsAsync()
    {
        IsLoading = true;
        Ships.Clear();

        try
        {
            var shipsFromServer = await ApiService.ApiClient.ShipAllAsync();
            foreach (var ship in shipsFromServer)
            {
                Ships.Add(ship);
            }
        }
        catch (ApiException apiEx)
        {
            MessageBox.Show($"Could not load ships. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
                "API Error");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task OnUpdateShipAsync()
    {
        var shipToEdit = new Ship
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

        var dialog = new MainMenuShipDialog(MapSpecToModel(shipToEdit));

        if (dialog.ShowDialog() == true)
        {
            IsLoading = true;
            try
            {
                if (dialog.Ship != null)
                {
                    var updatedShip =
                        await ApiService.ApiClient.ShipPUTAsync(dialog.Ship.Id, MapModelToSpec(dialog.Ship));

                    var index = Ships.IndexOf(CurrentShip);
                    if (index != -1)
                    {
                        Ships[index] = updatedShip;
                    }
                }
            }
            catch (ApiException apiEx)
            {
                MessageBox.Show($"Could not update ship. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
                    "API Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private async Task OnNewShipAsync()
    {
        var dialog = new MainMenuShipDialog(MapSpecToModel(new Ship()));

        if (dialog.ShowDialog() == true)
        {
            IsLoading = true;
            try
            {
                if (dialog.Ship != null)
                {
                    var newShip = await ApiService.ApiClient.ShipPOSTAsync(MapModelToSpec(dialog.Ship));

                    Ships.Add(newShip);
                }
            }
            catch (ApiException apiEx)
            {
                MessageBox.Show($"Could not create ship. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
                    "API Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private static Shared.Models.Ship MapSpecToModel(Ship spec) =>
        new()
        {
            Id = spec.Id,
            Name = spec.Name,
            ImoNumber = spec.ImoNumber,
            MmsiNumber = spec.MmsiNumber,
            Type = spec.Type,
            YearBuild = spec.YearBuild,
            LengthOverall = spec.LengthOverall,
            GrossTonnage = spec.GrossTonnage
        };

    private static Ship MapModelToSpec(Shared.Models.Ship model) =>
        new()
        {
            Id = model.Id,
            Name = model.Name,
            ImoNumber = model.ImoNumber,
            MmsiNumber = model.MmsiNumber,
            Type = model.Type,
            YearBuild = model.YearBuild,
            LengthOverall = model.LengthOverall,
            GrossTonnage = model.GrossTonnage
        };
}