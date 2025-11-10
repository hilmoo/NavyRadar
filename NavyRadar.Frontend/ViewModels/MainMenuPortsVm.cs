using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Frontend.Views.Dialog;
using NavyRadar.Shared.Spec;

namespace NavyRadar.Frontend.ViewModels;

public class MainMenuPortsVm : ObservableObject
{
    public ObservableCollection<Port> Ports { get; } = [];

    public Port CurrentPort
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = null!;

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


    public ICommand UpdatePortCommand { get; }
    public ICommand NewPortCommand { get; }
    public ICommand RefreshCommand { get; }

    public MainMenuPortsVm()
    {
        UpdatePortCommand = new SimpleRelayCommand(async void () =>
            {
                try
                {
                    await OnUpdatePortAsync();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error");
                }
            },
            () => !IsLoading);
        NewPortCommand = new SimpleRelayCommand(async void () =>
        {
            try
            {
                await OnNewPortAsync();
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
                await LoadPortsAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error");
            }
        }, () => !IsLoading);

        _ = LoadPortsAsync();
    }


    private async Task LoadPortsAsync()
    {
        IsLoading = true;
        Ports.Clear();

        try
        {
            var portsFromServer = await ApiService.ApiClient.PortsAllAsync();
            foreach (var port in portsFromServer)
            {
                Ports.Add(port);
            }
        }
        catch (ApiException apiEx)
        {
            MessageBox.Show($"Could not load ports. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
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

    private async Task OnUpdatePortAsync()
    {
        var portToEdit = new Port
        {
            Id = CurrentPort.Id,
            Name = CurrentPort.Name,
            CountryCode = CurrentPort.CountryCode,
            Location = new NpgsqlPoint { X = CurrentPort.Location.X, Y = CurrentPort.Location.Y }
        };

        var dialog = new MainMenuPortDialog(MapSpecToModel(portToEdit));

        if (dialog.ShowDialog() == true)
        {
            IsLoading = true;
            try
            {
                if (dialog.Port != null)
                {
                    var updatedPort =
                        await ApiService.ApiClient.PortsPUTAsync(dialog.Port.Id, MapModelToSpec(dialog.Port));

                    var index = Ports.IndexOf(CurrentPort);
                    if (index != -1)
                    {
                        Ports[index] = updatedPort;
                    }
                }
            }
            catch (ApiException apiEx)
            {
                MessageBox.Show(
                    $"Could not update port. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
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

    private async Task OnNewPortAsync()
    {
        var dialog = new MainMenuPortDialog(MapSpecToModel(new Port()));

        if (dialog.ShowDialog() == true)
        {
            IsLoading = true;
            try
            {
                if (dialog.Port != null)
                {
                    var newPort = await ApiService.ApiClient.PortsPOSTAsync(MapModelToSpec(dialog.Port));

                    Ports.Add(newPort);
                }
            }
            catch (ApiException apiEx)
            {
                MessageBox.Show(
                    $"Could not create port. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
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

    private static Shared.Models.Port MapSpecToModel(Port spec) =>
        new()
        {
            Id = spec.Id,
            Name = spec.Name,
            CountryCode = spec.CountryCode,
            Location = new NpgsqlTypes.NpgsqlPoint(spec.Location.X, spec.Location.Y)
        };

    private static Port MapModelToSpec(Shared.Models.Port model) =>
        new()
        {
            Id = model.Id,
            Name = model.Name,
            CountryCode = model.CountryCode,
            Location = new NpgsqlPoint { X = model.Location.X, Y = model.Location.Y }
        };
}