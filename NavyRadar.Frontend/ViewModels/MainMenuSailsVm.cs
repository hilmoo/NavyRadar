using System.Collections.ObjectModel;
using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Frontend.Views.Dialog;
using NavyRadar.Shared.Domain.Sail;
using NavyRadar.Shared.Entities;
using SailWithNameEntity = NavyRadar.Shared.Domain.Sail.SailWithName;

namespace NavyRadar.Frontend.ViewModels;

public class MainMenuSailsVm : ViewModelBase
{
    public ObservableCollection<SailWithNameEntity> Sails { get; } = [];

    public SailWithNameEntity CurrentSail
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

    public ICommand UpdateSailCommand { get; }
    public ICommand NewSailCommand { get; }
    public ICommand RemoveSailCommand { get; }

    public MainMenuSailsVm()
    {
        UpdateSailCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnUpdateSailAsync),
            () => !IsLoading);

        NewSailCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnNewSailAsync),
            () => !IsLoading);

        RemoveSailCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnRemoveSailAsync),
            () => !IsLoading);

        _ = ExecuteLoadingTask(LoadSailsAsync);
    }


    private async Task LoadSailsAsync()
    {
        Sails.Clear();

        var sailsFromServer = await ApiService.ApiClient.SailsAllAsync();
        foreach (var sail in sailsFromServer)
        {
            Sails.Add(sail.ToEntity());
        }

        CurrentSail = Sails.FirstOrDefault()!;
    }

    private async Task OnUpdateSailAsync()
    {
        var sailToEdit = new SailWithNameEntity
        {
            Id = CurrentSail.Id,
            ShipId = CurrentSail.ShipId,
            CaptainId = CurrentSail.CaptainId,
            OriginPortId = CurrentSail.OriginPortId,
            DestinationPortId = CurrentSail.DestinationPortId,
            Status = CurrentSail.Status,
            DepartureTime = CurrentSail.DepartureTime,
            ArrivalTime = CurrentSail.ArrivalTime,
            TotalDistanceNm = CurrentSail.TotalDistanceNm,
            AverageSpeedKnots = CurrentSail.AverageSpeedKnots,
            MaxSpeedKnots = CurrentSail.MaxSpeedKnots
        };

        var dialog = new MainMenuSailDialog(sailToEdit);

        if (dialog.ShowDialog() == true)
        {
            if (dialog.Sail != null)
            {
                var updatedSail =
                    await ApiService.ApiClient.SailsPUTAsync(dialog.Sail.Id, dialog.Sail.ToDto());

                var originalSailInList = Sails.FirstOrDefault(s => s.Id == updatedSail.Id);
                var updatedSailEntity = updatedSail.ToEntity();

                if (originalSailInList != null)
                {
                    var index = Sails.IndexOf(originalSailInList);

                    if (index != -1)
                    {
                        Sails[index] = updatedSailEntity;
                        CurrentSail = updatedSailEntity;
                    }
                }
            }
        }
    }

    private async Task OnNewSailAsync()
    {
        var dialog = new MainMenuSailDialog(null);

        if (dialog.ShowDialog() == true)
        {
            if (dialog.Sail != null)
            {
                var newSail = await ApiService.ApiClient.SailsPOSTAsync(dialog.Sail.ToDto());

                Sails.Add(newSail.ToEntity());
            }
        }
    }

    private async Task OnRemoveSailAsync()
    {
        var confirmed = ConfirmationRequested?.Invoke(
            "Confirm Removal",
            $"Are you sure you want to remove Sail ID: {CurrentSail.Id}? This action cannot be undone.") ?? false;

        if (!confirmed) return;

        await ApiService.ApiClient.SailsDELETEAsync(CurrentSail.Id);
        Sails.Remove(CurrentSail);
        CurrentSail = null!;
    }
}