using System.Windows;
using System.Windows.Controls;
using NavyRadar.Shared;
using NavyRadar.Shared.Entities;


namespace NavyRadar.Frontend.Views.Dialog;

public partial class MainMenuShipDialog
{
    public Ship? Ship { get; private set; }
    private readonly bool _isUpdateMode;

    public MainMenuShipDialog(Ship? ship)
    {
        InitializeComponent();

        TypeComboBox.ItemsSource = Enum.GetValues<ShipType>()
            .Select(st => new KeyValuePair<ShipType, string>(st, st.GetDescription()))
            .ToList();

        if (ship == null)
        {
            Title = "Add New Ship";
            Ship = null;
            _isUpdateMode = false;
        }
        else
        {
            Title = "Update Existing Ship";
            Ship = ship;
            _isUpdateMode = true;
            PopulateFields(ship);
        }
    }

    private void PopulateFields(Ship ship)
    {
        ImoNumberTextBox.Text = ship.ImoNumber;
        MmsiNumberTextBox.Text = ship.MmsiNumber;
        NameTextBox.Text = ship.Name;
        YearBuildTextBox.Text = ship.YearBuild.ToString();
        LengthTextBox.Text = ship.LengthOverall.ToString();
        GrossTonnageTextBox.Text = ship.GrossTonnage.ToString();
        TypeComboBox.SelectedValue = ship.Type;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
            string.IsNullOrWhiteSpace(ImoNumberTextBox.Text) ||
            string.IsNullOrWhiteSpace(MmsiNumberTextBox.Text) ||
            TypeComboBox.SelectedValue == null)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ShowValidationError("'Name' is a required field.", NameTextBox);
            }
            else if (string.IsNullOrWhiteSpace(ImoNumberTextBox.Text))
            {
                ShowValidationError("'IMO Number' is a required field.", ImoNumberTextBox);
            }
            else if (string.IsNullOrWhiteSpace(MmsiNumberTextBox.Text))
            {
                ShowValidationError("'MMSI Number' is a required field.", MmsiNumberTextBox);
            }
            else
            {
                ShowValidationError("'Type' is a required field.", TypeComboBox);
            }

            return;
        }

        if (!int.TryParse(YearBuildTextBox.Text, out var yearBuild) || yearBuild <= 1800)
        {
            ShowValidationError("'Year Build' must be a valid, realistic year (e.g., after 1800).", YearBuildTextBox);
            return;
        }

        if (!int.TryParse(LengthTextBox.Text, out var length) || length <= 0)
        {
            ShowValidationError("'Length Overall' must be a valid, positive number.", LengthTextBox);
            return;
        }

        if (!int.TryParse(GrossTonnageTextBox.Text, out var tonnage) || tonnage <= 0)
        {
            ShowValidationError("'Gross Tonnage' must be a valid, positive number.", GrossTonnageTextBox);
            return;
        }

        var shipType = (ShipType)TypeComboBox.SelectedValue;

        if (!_isUpdateMode)
        {
            Ship = new Ship
            {
                Name = NameTextBox.Text.Trim(),
                ImoNumber = ImoNumberTextBox.Text.Trim(),
                MmsiNumber = MmsiNumberTextBox.Text.Trim(),
                Type = shipType,
                YearBuild = yearBuild,
                LengthOverall = length,
                GrossTonnage = tonnage
            };
        }
        else if (Ship != null)
        {
            Ship.Name = NameTextBox.Text.Trim();
            Ship.ImoNumber = ImoNumberTextBox.Text.Trim();
            Ship.MmsiNumber = MmsiNumberTextBox.Text.Trim();
            Ship.Type = shipType;
            Ship.YearBuild = yearBuild;
            Ship.LengthOverall = length;
            Ship.GrossTonnage = tonnage;
        }

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private static void ShowValidationError(string message, Control controlToFocus)
    {
        MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        controlToFocus.Focus();
    }
}