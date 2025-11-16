using System.Windows;

namespace NavyRadar.Frontend.Views.Dialog;

public partial class MainMenuSailingAddPosition
{
    public double PositionLatitude { get; private set; }
    public double PositionLongitude { get; private set; }
    public double SpeedKnots { get; private set; }
    public int HeadingDegrees { get; private set; }

    public MainMenuSailingAddPosition()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (!double.TryParse(LatitudeTextBox.Text, out var lat))
        {
            MessageBox.Show("Please enter a valid number for Latitude.", "Invalid Input", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!double.TryParse(LongitudeTextBox.Text, out var lon))
        {
            MessageBox.Show("Please enter a valid number for Longitude.", "Invalid Input", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!double.TryParse(SpeedTextBox.Text, out var speed))
        {
            MessageBox.Show("Please enter a valid number for Speed.", "Invalid Input", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(HeadingTextBox.Text, out var heading))
        {
            MessageBox.Show("Please enter a valid whole number for Heading.", "Invalid Input", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        PositionLatitude = lat;
        PositionLongitude = lon;
        SpeedKnots = speed;
        HeadingDegrees = heading;

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}