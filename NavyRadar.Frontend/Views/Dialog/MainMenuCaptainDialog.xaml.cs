using System.Windows;

namespace NavyRadar.Frontend.Views.Dialog;

public partial class MainMenuCaptainDialog
{
    public string FirstName { get; private set; } = "";
    public string LastName { get; private set; } = "";
    public string LicenseNumber { get; private set; } = "";

    public MainMenuCaptainDialog()
    {
        InitializeComponent();
        Owner = Application.Current.MainWindow;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
        {
            MessageBox.Show("'First Name' is a required field.", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            FirstNameTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
        {
            MessageBox.Show("'Last Name' is a required field.", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            LastNameTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(LicenseTextBox.Text))
        {
            MessageBox.Show("'License Number' is a required field.", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            LicenseTextBox.Focus();
            return;
        }

        FirstName = FirstNameTextBox.Text.Trim();
        LastName = LastNameTextBox.Text.Trim();
        LicenseNumber = LicenseTextBox.Text.Trim();

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}