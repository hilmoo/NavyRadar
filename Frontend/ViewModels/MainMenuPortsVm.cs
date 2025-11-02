using System.Diagnostics;
using System.Windows.Input;
using Frontend.Util;
using Frontend.Views.Dialog;
using NpgsqlTypes;
using Shared.Models;

namespace Frontend.ViewModels;

public class MainMenuPortsVm : ObservableObject
{
    public List<Port>? Ports { get; private set; }

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

    public ICommand UpdatePortCommand { get; }
    public ICommand NewPortCommand { get; }

    public MainMenuPortsVm()
    {
        LoadDummyData();
        UpdatePortCommand = new SimpleRelayCommand(OnUpdatePort);
        NewPortCommand = new SimpleRelayCommand(OnNewPort);
    }

    private void OnUpdatePort()
    {
        Debug.WriteLine(CurrentPort);
        var dialog = new MainMenuPortDialog(CurrentPort);

        if (dialog.ShowDialog() == true)
        {
            Debug.WriteLine("--- PORT UPDATE ---");
            Debug.WriteLine($"User clicked OK.");
            Debug.WriteLine($"Id:            {dialog.Port!.Id}");
            Debug.WriteLine($"Name:          {dialog.Port.Name}");
            Debug.WriteLine($"CountryCode:    {dialog.Port.CountryCode}");
            Debug.WriteLine($"Location:   {dialog.Port.Location}");
            Debug.WriteLine("------------------------");
        }
        else
        {
            Debug.WriteLine("--- PORT UPDATE ---");
            Debug.WriteLine("User clicked Cancel. No changes made.");
            Debug.WriteLine("------------------------");
        }
    }

    private static void OnNewPort()
    {
        var dialog = new MainMenuPortDialog(null);

        if (dialog.ShowDialog() == true)
        {
            Debug.WriteLine("--- PORT UPDATE ---");
            Debug.WriteLine($"User clicked OK.");
            Debug.WriteLine($"Id:            {dialog.Port!.Id}");
            Debug.WriteLine($"Name:          {dialog.Port.Name}");
            Debug.WriteLine($"CountryCode:    {dialog.Port.CountryCode}");
            Debug.WriteLine($"Location:   {dialog.Port.Location}");
            Debug.WriteLine("------------------------");
        }
        else
        {
            Debug.WriteLine("--- PORT UPDATE ---");
            Debug.WriteLine("User clicked Cancel. No changes made.");
            Debug.WriteLine("------------------------");
        }
    }

    private void LoadDummyData()
    {
        Ports =
        [
            new Port
            {
                Id = 1,
                Name = "Port of Singapore",
                CountryCode = "SG",
                Location = new NpgsqlPoint { X = 1.2644, Y = 103.8401 }
            },

            new Port
            {
                Id = 2,
                Name = "Port of Rotterdam",
                CountryCode = "NL",
                Location = new NpgsqlPoint { X = 51.9470, Y = 4.1399 }
            },

            new Port
            {
                Id = 3,
                Name = "Port of Los Angeles",
                CountryCode = "US",
                Location = new NpgsqlPoint { X = 33.7292, Y = -118.2620 }
            }
        ];
    }
}