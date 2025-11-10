using System.Windows;
using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Shared.Spec;
using LoginDto = NavyRadar.Shared.Domain.LoginDto;

namespace NavyRadar.Frontend.ViewModels;

public class SignInVm : ViewModelBase
{
    private readonly NavigationVm _navigationVm;

    public string Username
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;


    public string Password
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;

    public bool IsLoading
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public ICommand SignInCommand { get; }
    public ICommand HomeCommand => _navigationVm.HomeCommand;
    public ICommand RegisterCommand => _navigationVm.RegisterCommand;

    public SignInVm(NavigationVm navigationVm)
    {
        _navigationVm = navigationVm;

        SignInCommand = new SimpleRelayCommand(
            async void () =>
            {
                try
                {
                    await OnLoginAsync();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error");
                }
            },
            () => !IsLoading);
    }

    private async Task OnLoginAsync()
    {
        IsLoading = true;
        try
        {
            var loginDto = new LoginDto
            {
                Username = Username,
                Password = Password
            };
            var account = await ApiService.ApiClient.SigninAsync(MapModelToSpec(loginDto));
            ApiService.ApiClient.SetJwtToken(account.Token);

            _navigationVm.NavigateToMain(MapSpecToModel(account.UserAccount));
        }
        catch (ApiException apiEx)
        {
            if (apiEx.StatusCode == 401)
            {
                MessageBox.Show("Invalid username or password.", "Login Failed");
            }
            else
            {
                MessageBox.Show(
                    $"Could not log in. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
                    "API Error");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static Shared.Spec.LoginDto MapModelToSpec(LoginDto model) =>
        new()
        {
            Username = model.Username,
            Password = model.Password
        };

    private static Shared.Models.Account MapSpecToModel(Account spec) =>
        new()
        {
            Id = spec.Id,
            Username = spec.Username,
            Email = spec.Email,
            Password = spec.Password,
            Role = spec.Role
        };
}