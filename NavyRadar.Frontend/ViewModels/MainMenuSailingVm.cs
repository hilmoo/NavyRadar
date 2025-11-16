using System.Diagnostics;
using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Frontend.Views.Dialog;
using NavyRadar.Shared.Domain.Sail;
using NavyRadar.Shared.Entities;
using NavyRadar.Shared.Spec;
using SailWithNameEntity = NavyRadar.Shared.Domain.Sail.SailWithName;
using AccountEntity = NavyRadar.Shared.Entities.Account;

namespace NavyRadar.Frontend.ViewModels;

public class MainMenuSailingVm : ViewModelBase
{
    public SailWithNameEntity? CurrentSail
    {
        get;
        private set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasActiveSail));
        }
    }

    public bool HasActiveSail => CurrentSail != null;

    public AccountEntity CurrentAccount { get; init; } = null!;

    public event Func<string, string, bool>? ConfirmationRequested;
    public new event Action<string, string>? ErrorOccurred;
    public ICommand RefreshCommand { get; }
    public ICommand AddPositionCommand { get; }
    public ICommand CompleteSailCommand { get; }
    public ICommand ToggleSatusCommand { get; }

    public MainMenuSailingVm()
    {
        RefreshCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(LoadSailsAsync),
            () => !IsLoading);

        AddPositionCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnNewPositionAsync),
            () => !IsLoading && HasActiveSail);

        CompleteSailCommand = new SimpleRelayCommand(
            () => _ = CompleteSailAsync(),
            () => !IsLoading && HasActiveSail);

        ToggleSatusCommand = new SimpleRelayCommand(
            () => _ = OnToggleStatusAsync(),
            () => !IsLoading && HasActiveSail);

        _ = ExecuteLoadingTask(LoadSailsAsync);
    }

    private async Task LoadSailsAsync()
    {
        try
        {
            var sailDto = await ApiService.ApiClient.ActiveAsync();
            CurrentSail = sailDto.ToEntity();
        }
        catch (ApiException apiEx) when (apiEx.StatusCode == 404)
        {
            CurrentSail = null;
        }
        catch (ApiException apiEx)
        {
            Debug.WriteLine($"API Error: {apiEx.StatusCode} - {apiEx.Response}");
            ErrorOccurred?.Invoke(
                "API Error",
                $"Could not perform operation. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unexpected Error: {ex.Message}");
            ErrorOccurred?.Invoke(
                "Error",
                $"An unexpected error occurred: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static async Task OnNewPositionAsync()
    {
        var dialog = new MainMenuSailingAddPosition();

        if (dialog.ShowDialog() == true)
        {
            var request = new AddPositionRequest
            {
                Latitude = dialog.PositionLatitude,
                Longitude = dialog.PositionLongitude,
                SpeedKnots = dialog.SpeedKnots,
                HeadingDegrees = dialog.HeadingDegrees
            };

            await ApiService.ApiClient.PositionAsync(request);
        }
    }

    private async Task OnToggleStatusAsync()
    {
        if (CurrentSail == null) return;

        var newStatus = (CurrentSail.Status == SailStatus.Docked) ? SailStatus.Sailing : SailStatus.Docked;
        var newStatusText = newStatus.ToString();
        var currentStatusText = CurrentSail.Status.ToString();

        var confirmed = ConfirmationRequested?.Invoke(
                            "Confirm Status Change",
                            $"Are you sure you want to change the sail status from '{currentStatusText}' to '{newStatusText}'?") ??
                        false;

        if (!confirmed) return;

        await ApiService.ApiClient.StatusAsync(new UpdateSailStatusRequest
        {
            Status = (int)newStatus
        });

        await LoadSailsAsync();
    }

    private async Task CompleteSailAsync()
    {
        var confirmed = ConfirmationRequested?.Invoke(
                            "Confirm Complete Sail",
                            $"Are you sure you want to complete the current sail (ID: {CurrentSail?.Id})? This action cannot be undone.") ??
                        false;

        if (!confirmed) return;

        await ApiService.ApiClient.CompleteAsync();

        await LoadSailsAsync();
    }
}