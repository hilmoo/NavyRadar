using System.Windows;
using NavyRadar.Shared.Models;
using NavyRadar.Shared.Util;


namespace NavyRadar.Frontend.Views.Dialog;

public partial class MainMenuShipDialog
{
    public Ship? Ship { get; private set; }
    private readonly bool _isUpdateMode;

    public MainMenuShipDialog(Ship? ship)
    {
        InitializeComponent();

        TypeComboBox.ItemsSource = ShipType.ShipTypes;

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
        MmsiNumberTextBox.Text = ship.MmsiNumber ?? string.Empty;
        NameTextBox.Text = ship.Name;
        YearBuildTextBox.Text = ship.YearBuild?.ToString() ?? string.Empty;
        LengthTextBox.Text = ship.LengthOverall?.ToString() ?? string.Empty;
        GrossTonnageTextBox.Text = ship.GrossTonnage?.ToString() ?? string.Empty;

        if (ship.Type != null)
        {
            TypeComboBox.SelectedValue = ship.Type;
        }
        else
        {
            TypeComboBox.SelectedIndex = -1;
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            MessageBox.Show("'Name' is a required field.", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            NameTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(ImoNumberTextBox.Text))
        {
            MessageBox.Show("'IMO Number' is a required field.", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            ImoNumberTextBox.Focus();
            return;
        }

        int? yearBuild = int.TryParse(YearBuildTextBox.Text, out var year) ? year : null;
        int? length = int.TryParse(LengthTextBox.Text, out var len) ? len : null;
        int? tonnage = int.TryParse(GrossTonnageTextBox.Text, out var ton) ? ton : null;

        if (!_isUpdateMode)
        {
            Ship = new Ship
            {
                Name = NameTextBox.Text.Trim(),
                ImoNumber = ImoNumberTextBox.Text.Trim()
            };
        }

        if (Ship != null)
        {
            Ship.Name = NameTextBox.Text.Trim();
            Ship.ImoNumber = ImoNumberTextBox.Text.Trim();
            Ship.MmsiNumber =
                string.IsNullOrWhiteSpace(MmsiNumberTextBox.Text) ? null : MmsiNumberTextBox.Text.Trim();
            Ship.Type = TypeComboBox.SelectedValue as string;
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
}