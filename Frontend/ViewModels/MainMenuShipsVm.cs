using System.Diagnostics;
using System.Windows.Input;
using Frontend.Util;
using Frontend.Views.Dialog;
using Shared.Models;
using Shared.Util;

namespace Frontend.ViewModels;

public class MainMenuShipsVm : ObservableObject
{
    public List<Ship>? Ships { get; private set; }

    public Ship CurrentShip
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = null!;

    public ICommand UpdateShipCommand { get; }
    public ICommand NewShipCommand { get; }

    public MainMenuShipsVm()
    {
        LoadDummyData();
        UpdateShipCommand = new SimpleRelayCommand(OnUpdateShip);
        NewShipCommand = new SimpleRelayCommand(OnNewShip);
    }

    private void LoadDummyData()
    {
        Ships =
        [
            new Ship
            {
                Id = 1,
                ImoNumber = "IMO1234567",
                MmsiNumber = "123456789",
                Name = "Ever Given",
                Type = nameof(ShipType.CargoVessels),
                YearBuild = 2018,
                LengthOverall = 400,
                GrossTonnage = 220940
            },

            new Ship
            {
                Id = 2,
                ImoNumber = "IMO7654321",
                MmsiNumber = "987654321",
                Name = "Symphony of the Seas",
                Type = nameof(ShipType.CargoVessels),
                YearBuild = 2018,
                LengthOverall = 361,
                GrossTonnage = 228081
            },

            new Ship
            {
                Id = 3,
                ImoNumber = "IMO1122334",
                MmsiNumber = "112233445",
                Name = "Queen Mary 2",
                Type = nameof(ShipType.CargoVessels),
                YearBuild = 2004,
                LengthOverall = 345,
                GrossTonnage = 148528
            }
        ];
    }

    private void OnUpdateShip()
    {
        Debug.WriteLine(CurrentShip);
        var dialog = new MainMenuShipDialog(CurrentShip);

        if (dialog.ShowDialog() == true)
        {
            Debug.WriteLine("--- SHIP UPDATE ---");
            Debug.WriteLine($"User clicked OK.");
            Debug.WriteLine($"Id:            {dialog.Ship!.Id}");
            Debug.WriteLine($"Name:          {dialog.Ship.Name}");
            Debug.WriteLine($"IMO Number:    {dialog.Ship.ImoNumber}");
            Debug.WriteLine($"MMSI Number:   {dialog.Ship.MmsiNumber}");
            Debug.WriteLine($"Type:          {dialog.Ship.Type}");
            Debug.WriteLine($"Year Build:    {dialog.Ship.YearBuild}");
            Debug.WriteLine($"Length:        {dialog.Ship.LengthOverall}");
            Debug.WriteLine($"Gross Tonnage: {dialog.Ship.GrossTonnage}");
            Debug.WriteLine("------------------------");
        }
        else
        {
            Debug.WriteLine("--- SHIP UPDATE ---");
            Debug.WriteLine("User clicked Cancel. No changes made.");
            Debug.WriteLine("------------------------");
        }
    }

    private static void OnNewShip()
    {
        var dialog = new MainMenuShipDialog(null);

        if (dialog.ShowDialog() == true)
        {
            Debug.WriteLine("--- SHIP UPDATE ---");
            Debug.WriteLine($"User clicked OK.");
            Debug.WriteLine($"Id:            {dialog.Ship!.Id}");
            Debug.WriteLine($"Name:          {dialog.Ship.Name}");
            Debug.WriteLine($"IMO Number:    {dialog.Ship.ImoNumber}");
            Debug.WriteLine($"MMSI Number:   {dialog.Ship.MmsiNumber}");
            Debug.WriteLine($"Type:          {dialog.Ship.Type}");
            Debug.WriteLine($"Year Build:    {dialog.Ship.YearBuild}");
            Debug.WriteLine($"Length:        {dialog.Ship.LengthOverall}");
            Debug.WriteLine($"Gross Tonnage: {dialog.Ship.GrossTonnage}");
            Debug.WriteLine("------------------------");
        }
        else
        {
            Debug.WriteLine("--- SHIP NEW ---");
            Debug.WriteLine("User clicked Cancel. No changes made.");
            Debug.WriteLine("------------------------");
        }
    }
}