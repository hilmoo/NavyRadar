using System.Windows;

namespace Frontend.Views.Dialog;

public partial class MainMenuMyAccountDialog
{
    public string UpdatedUsername { get; private set; } = null!;
    public string UpdatedEmail { get; private set; } = null!;
    public string UpdatedPassword { get; private set; } = null!;

    public MainMenuMyAccountDialog(string currentUsername, string currentEmail)
    {
        InitializeComponent();

        UsernameTextBox.Text = currentUsername;
        EmailTextBox.Text = currentEmail;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        UpdatedUsername = UsernameTextBox.Text;
        UpdatedEmail = EmailTextBox.Text;
        UpdatedPassword = PasswordTextBox.Password;

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}