using System.Globalization;
using System.Windows;
using NpgsqlTypes;
using NavyRadar.Shared.Models;

namespace NavyRadar.Frontend.Views.Dialog;

public partial class MainMenuPortDialog
{
    public Port? Port { get; private set; }
    private readonly bool _isUpdateMode;

    public MainMenuPortDialog(Port? port)
    {
        InitializeComponent();

        if (port == null)
        {
            Title = "Add New Port";
            Port = null;
            _isUpdateMode = false;
        }
        else
        {
            Title = "Update Existing Port";
            Port = port;
            _isUpdateMode = true;
            PopulateFields(port);
        }
    }

    private void PopulateFields(Port port)
    {
        NameTextBox.Text = port.Name;
        CountryCodeTextBox.Text = port.CountryCode;
        LatitudeTextBox.Text = port.Location.X.ToString(CultureInfo.InvariantCulture);
        LongitudeTextBox.Text = port.Location.Y.ToString(CultureInfo.InvariantCulture);
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

        if (string.IsNullOrWhiteSpace(CountryCodeTextBox.Text))
        {
            MessageBox.Show("'Country Code' is a required field.", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            CountryCodeTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(LatitudeTextBox.Text))
        {
            MessageBox.Show("'Latitude' is a required field.", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            LatitudeTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(LongitudeTextBox.Text))
        {
            MessageBox.Show("'Longitude' is a required field.", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            LongitudeTextBox.Focus();
            return;
        }

        if (!double.TryParse(LatitudeTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
        {
            MessageBox.Show("Invalid 'Latitude' value. Please enter a valid number.", "Validation Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            LatitudeTextBox.Focus();
            return;
        }

        if (!double.TryParse(LongitudeTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
        {
            MessageBox.Show("Invalid 'Longitude' value. Please enter a valid number.", "Validation Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            LongitudeTextBox.Focus();
            return;
        }

        var name = NameTextBox.Text.Trim();
        var countryCode = CountryCodeTextBox.Text.Trim();

        if (_isUpdateMode)
        {
            if (Port != null)
            {
                Port.Name = name;
                Port.CountryCode = countryCode;
                Port.Location = Port.Location with { X = lat };
                Port.Location = Port.Location with { Y = lon };
            }
        }
        else
        {
            Port = new Port
            {
                Name = name,
                CountryCode = countryCode,
                Location = new NpgsqlPoint { X = lat, Y = lon }
            };
        }

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}