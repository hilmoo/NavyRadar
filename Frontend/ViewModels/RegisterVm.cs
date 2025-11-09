using System.Windows;
using System.Windows.Input;
using Frontend.Util;
using Shared.Spec;
using RegisterDto = Shared.Domain.RegisterDto;

namespace Frontend.ViewModels;

public class RegisterVm : ViewModelBase
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

    public string Email
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

    public ICommand RegisterCommand { get; }
    public ICommand HomeCommand => _navigationVm.HomeCommand;
    public ICommand SignInCommand => _navigationVm.SignInCommand;

    public RegisterVm(NavigationVm navigationVm)
    {
        _navigationVm = navigationVm;

        RegisterCommand = new SimpleRelayCommand(
            async void () =>
            {
                try
                {
                    await OnRegisterAsync();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An unexpected error occurred: {e.Message}", "Error");
                }
            },
            () => !IsLoading);
    }

    private async Task OnRegisterAsync()
    {
        IsLoading = true;
        try
        {
            var registerDto = new RegisterDto()
            {
                Username = Username,
                Password = Password,
                Email = Email
            };
            var account = await ApiService.ApiClient.RegisterAsync(MapModelToSpec(registerDto));
            ApiService.ApiClient.SetJwtToken(account.Token);

            _navigationVm.NavigateToMain(MapSpecToModel(account.UserAccount));
        }
        catch (ApiException apiEx)
        {
            MessageBox.Show(
                $"Could not register. Server responded with {apiEx.StatusCode}.\n{apiEx.Response}",
                "API Error");
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

    private static Shared.Spec.RegisterDto MapModelToSpec(RegisterDto model) =>
        new()
        {
            Username = model.Username,
            Password = model.Password,
            Email = model.Email
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