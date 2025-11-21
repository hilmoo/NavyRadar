using System.Collections.ObjectModel;
using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Frontend.Views.Dialog;
using NavyRadar.Shared.Entities;
using PortEntity = NavyRadar.Shared.Entities.Port;

namespace NavyRadar.Frontend.ViewModels;

public class MainMenuPortsVm : ViewModelBase
{
    public ObservableCollection<PortEntity> Ports { get; } = [];

    public PortEntity CurrentPort
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = null!;

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

    public ICommand UpdatePortCommand { get; }
    public ICommand NewPortCommand { get; }
    public ICommand RemovePortCommand { get; }

    public MainMenuPortsVm()
    {
        UpdatePortCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnUpdatePortAsync),
            () => !IsLoading);

        NewPortCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnNewPortAsync),
            () => !IsLoading);

        RemovePortCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnRemovePortAsync),
            () => !IsLoading);

        _ = ExecuteLoadingTask(LoadPortsAsync);
    }


    private async Task LoadPortsAsync()
    {
        Ports.Clear();

        var portsFromServer = await ApiService.ApiClient.PortsAllAsync();
        foreach (var port in portsFromServer)
        {
            Ports.Add(port.ToEntity());
        }

        CurrentPort = Ports.FirstOrDefault()!;
    }

    private async Task OnUpdatePortAsync()
    {
        var portToEdit = new PortEntity
        {
            Id = CurrentPort.Id,
            Name = CurrentPort.Name,
            CountryCode = CurrentPort.CountryCode,
            Location = new NpgsqlTypes.NpgsqlPoint { X = CurrentPort.Location.X, Y = CurrentPort.Location.Y }
        };

        var dialog = new MainMenuPortDialog(portToEdit);

        if (dialog.ShowDialog() == true)
        {
            if (dialog.Port != null)
            {
                var updatedPort =
                    await ApiService.ApiClient.PortsPUTAsync(dialog.Port.Id, dialog.Port.ToDto());

                var originalPortInList = Ports.FirstOrDefault(s => s.Id == updatedPort.Id);
                var updatedPortEntity = updatedPort.ToEntity();

                if (originalPortInList != null)
                {
                    var index = Ports.IndexOf(originalPortInList);

                    if (index != -1)
                    {
                        Ports[index] = updatedPortEntity;
                        CurrentPort = updatedPortEntity;
                    }
                }
            }
        }
    }

    private async Task OnNewPortAsync()
    {
        var dialog = new MainMenuPortDialog(new PortEntity
        {
            Name = "",
            CountryCode = "",
            Location = new NpgsqlTypes.NpgsqlPoint { X = 0, Y = 0 }
        });

        if (dialog.ShowDialog() == true)
        {
            if (dialog.Port != null)
            {
                var newPort = await ApiService.ApiClient.PortsPOSTAsync(dialog.Port.ToDto());

                Ports.Add(newPort.ToEntity());
            }
        }
    }

    private async Task OnRemovePortAsync()
    {
        var confirmed = ConfirmationRequested?.Invoke(
            "Confirm Removal",
            $"Are you sure you want to remove {CurrentPort.Name}? This action cannot be undone.") ?? false;

        if (!confirmed) return;

        await ApiService.ApiClient.PortsDELETEAsync(CurrentPort.Id);
        Ports.Remove(CurrentPort);
        CurrentPort = null!;
    }
}