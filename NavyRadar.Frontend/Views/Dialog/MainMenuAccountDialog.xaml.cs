using System.Windows;
using NavyRadar.Shared.Util;
using NavyRadar.Shared.Models;

namespace NavyRadar.Frontend.Views.Dialog;

public partial class MainMenuAccountDialog
{
    public Account? Account { get; private set; }
    private readonly bool _isUpdateMode;

    public MainMenuAccountDialog(Account? account)
    {
        InitializeComponent();

        RoleComboBox.ItemsSource = Enum.GetValues<RoleType>();

        if (account == null)
        {
            Title = "Add New User";
            Account = null;
            _isUpdateMode = false;
        }
        else
        {
            Title = "Update Existing User";
            Account = account;
            _isUpdateMode = true;
            PopulateFields(account);
        }
    }

    private void PopulateFields(Account account)
    {
        UsernameTextBox.Text = account.Username;
        PasswordTextBox.Password = "";
        EmailTextBox.Text = account.Email;

        if (Enum.TryParse<RoleType>(account.Role, out var accountRole))
        {
            RoleComboBox.SelectedItem = accountRole;
        }
        else
        {
            RoleComboBox.SelectedIndex = -1;
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
        {
            MessageBox.Show("'Username' is a required field.", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            UsernameTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordTextBox.Password) && !_isUpdateMode)
        {
            MessageBox.Show("'Password' is a required field.", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            PasswordTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
        {
            MessageBox.Show("'Email' is a required field.", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            EmailTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(RoleComboBox.Text))
        {
            MessageBox.Show("'Role' is a required field.", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            RoleComboBox.Focus();
            return;
        }

        var username = UsernameTextBox.Text.Trim();
        var password = PasswordTextBox.Password.Trim();
        var email = EmailTextBox.Text.Trim();
        var role = RoleComboBox.Text.Trim();

        if (_isUpdateMode)
        {
            if (Account != null)
            {
                Account.Username = username;
                Account.Password = password == "" ? Account.Password : password;
                Account.Email = email;
                Account.Role = role;
            }
        }
        else
        {
            Account = new Account
            {
                Username = username,
                Password = password,
                Email = email,
                Role = role
            };
        }

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}