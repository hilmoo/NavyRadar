using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Frontend.Views.Dialog;

public partial class MainMenuSailDialog
{
    public Sail? Sail { get; private set; }
    private readonly bool _isUpdateMode;

    public MainMenuSailDialog(Sail? sail)
    {
        InitializeComponent();

        if (sail == null)
        {
            Title = "Add New Sail";
            Sail = null;
            _isUpdateMode = false;

            var now = DateTime.Now.ToUniversalTime();
            DepartureTimePicker.SelectedDate = now.Date;
            DepartureHourTextBox.Text = now.Hour.ToString("D2");
            DepartureMinuteTextBox.Text = now.Minute.ToString("D2");
            StatusComboBox.ItemsSource = new List<SailStatus>
            {
                SailStatus.Docked,
                SailStatus.Sailing
            };
            StatusComboBox.SelectedItem = SailStatus.Docked;
            UpdateFieldEnabledState(SailStatus.Docked);
        }
        else
        {
            Title = "Update Existing Sail";
            Sail = sail;
            _isUpdateMode = true;
            PopulateFields(sail);
            StatusComboBox.ItemsSource = new List<SailStatus>
            {
                SailStatus.Finished,
                SailStatus.Cancelled
            };
            UpdateFieldEnabledState(sail.Status);
        }
    }

    private void PopulateFields(Sail sail)
    {
        ShipIdTextBox.Text = sail.ShipId.ToString();
        CaptainIdTextBox.Text = sail.CaptainId.ToString();
        OriginPortIdTextBox.Text = sail.OriginPortId.ToString();
        DestinationPortIdTextBox.Text = sail.DestinationPortId.ToString();
        StatusComboBox.SelectedItem = sail.Status;

        DepartureTimePicker.SelectedDate = sail.DepartureTime.Date;
        DepartureHourTextBox.Text = sail.DepartureTime.Hour.ToString("D2");
        DepartureMinuteTextBox.Text = sail.DepartureTime.Minute.ToString("D2");

        if (sail.ArrivalTime.HasValue)
        {
            ArrivalTimePicker.SelectedDate = sail.ArrivalTime.Value.Date;
            ArrivalHourTextBox.Text = sail.ArrivalTime.Value.Hour.ToString("D2");
            ArrivalMinuteTextBox.Text = sail.ArrivalTime.Value.Minute.ToString("D2");
        }

        TotalDistanceNmTextBox.Text = sail.TotalDistanceNm?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        AverageSpeedKnotsTextBox.Text = sail.AverageSpeedKnots?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        MaxSpeedKnotsTextBox.Text = sail.MaxSpeedKnots?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInt(ShipIdTextBox, "Ship ID", out var shipId)) return;
        if (!ValidateInt(CaptainIdTextBox, "Captain ID", out var captainId)) return;
        if (!ValidateInt(OriginPortIdTextBox, "Origin Port ID", out var originPortId)) return;
        if (!ValidateInt(DestinationPortIdTextBox, "Destination Port ID", out var destPortId)) return;

        if (StatusComboBox.SelectedItem == null)
        {
            ShowError("Please select a 'Status'.", StatusComboBox);
            return;
        }

        var status = (SailStatus)StatusComboBox.SelectedItem;

        if (!ParseDateTime(
                DepartureTimePicker, DepartureHourTextBox, DepartureMinuteTextBox,
                "Departure Time", isRequired: true, out var departureTime))
        {
            return;
        }

        if (!ParseDateTime(
                ArrivalTimePicker, ArrivalHourTextBox, ArrivalMinuteTextBox,
                "Arrival Time", isRequired: false, out var arrivalTime))
        {
            return;
        }

        if (!ParseNullableDouble(TotalDistanceNmTextBox, "Total Distance", out var totalDistance)) return;
        if (!ParseNullableDouble(AverageSpeedKnotsTextBox, "Average Speed", out var avgSpeed)) return;
        if (!ParseNullableDouble(MaxSpeedKnotsTextBox, "Max Speed", out var maxSpeed)) return;


        if (_isUpdateMode)
        {
            if (Sail != null)
            {
                Sail.ShipId = shipId;
                Sail.CaptainId = captainId;
                Sail.OriginPortId = originPortId;
                Sail.DestinationPortId = destPortId;
                Sail.Status = status;
                Sail.DepartureTime = departureTime!.Value;
                Sail.ArrivalTime = arrivalTime;
                Sail.TotalDistanceNm = totalDistance;
                Sail.AverageSpeedKnots = avgSpeed;
                Sail.MaxSpeedKnots = maxSpeed;
            }
        }
        else
        {
            Sail = new Sail
            {
                ShipId = shipId,
                CaptainId = captainId,
                OriginPortId = originPortId,
                DestinationPortId = destPortId,
                Status = status,
                DepartureTime = departureTime!.Value,
                ArrivalTime = null,
                TotalDistanceNm = null,
                AverageSpeedKnots = null,
                MaxSpeedKnots = null
            };
        }

        DialogResult = true;
    }

    private bool ParseDateTime(DatePicker datePicker, TextBox hourBox, TextBox minBox, string fieldName,
        bool isRequired, out DateTime? outDateTime)
    {
        outDateTime = null;

        if (datePicker.SelectedDate == null)
        {
            if (!isRequired)
            {
                hourBox.Text = string.Empty;
                minBox.Text = string.Empty;
                return true;
            }

            ShowError($"'{fieldName}' date is a required field.", datePicker);
            return false;
        }

        var datePart = datePicker.SelectedDate.Value.Date;

        if (!isRequired && string.IsNullOrWhiteSpace(hourBox.Text) && string.IsNullOrWhiteSpace(minBox.Text))
        {
            outDateTime = datePart;
            return true;
        }

        if (!int.TryParse(hourBox.Text, out var hour))
        {
            ShowError($"Invalid '{fieldName}' hour. Must be a number.", hourBox);
            return false;
        }

        if (hour is < 0 or > 23)
        {
            ShowError($"Invalid '{fieldName}' hour. Must be between 0 and 23.", hourBox);
            return false;
        }

        if (!int.TryParse(minBox.Text, out var minute))
        {
            ShowError($"Invalid '{fieldName}' minute. Must be a number.", minBox);
            return false;
        }

        if (minute is < 0 or > 59)
        {
            ShowError($"Invalid '{fieldName}' minute. Must be between 0 and 59.", minBox);
            return false;
        }

        try
        {
            outDateTime = datePart.AddHours(hour).AddMinutes(minute);
            return true;
        }
        catch (Exception ex)
        {
            ShowError($"Error combining '{fieldName}': {ex.Message}", datePicker);
            return false;
        }
    }

    private bool ValidateInt(TextBox textBox, string fieldName, out int value)
    {
        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            ShowError($"'{fieldName}' is a required field.", textBox);
            value = 0;
            return false;
        }

        if (int.TryParse(textBox.Text, out value)) return true;
        ShowError($"Invalid '{fieldName}' value. Please enter a valid number.", textBox);
        return false;
    }

    private bool ParseNullableDouble(TextBox textBox, string fieldName, out double? value)
    {
        value = null;
        var text = textBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedValue))
        {
            ShowError($"Invalid '{fieldName}' value. Please enter a valid number.", textBox);
            return false;
        }

        value = parsedValue;
        return true;
    }

    private static void ShowError(string message, Control control)
    {
        MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        control.Focus();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void UpdateFieldEnabledState(SailStatus status)
    {
        var isCompleted = status == SailStatus.Finished;

        ArrivalTimePicker.IsEnabled = isCompleted;
        ArrivalHourTextBox.IsEnabled = isCompleted;
        ArrivalMinuteTextBox.IsEnabled = isCompleted;
        TotalDistanceNmTextBox.IsEnabled = isCompleted;
        AverageSpeedKnotsTextBox.IsEnabled = isCompleted;
        MaxSpeedKnotsTextBox.IsEnabled = isCompleted;

        if (isCompleted) return;
        ArrivalTimePicker.SelectedDate = null;
        ArrivalHourTextBox.Text = string.Empty;
        ArrivalMinuteTextBox.Text = string.Empty;
        TotalDistanceNmTextBox.Text = string.Empty;
        AverageSpeedKnotsTextBox.Text = string.Empty;
        MaxSpeedKnotsTextBox.Text = string.Empty;
    }
}