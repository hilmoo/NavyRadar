using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NavyRadar.Shared.Spec;

namespace NavyRadar.Frontend.Util;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName ?? string.Empty));
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public bool IsLoading
    {
        get;
        set => SetProperty(ref field, value);
    }

    public event Action<string, string>? ErrorOccurred;

    protected async Task ExecuteLoadingTask(Func<Task> action)
    {
        IsLoading = true;
        try
        {
            await action();
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
}