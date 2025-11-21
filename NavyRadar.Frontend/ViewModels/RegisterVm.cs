using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Shared.Entities;
using NavyRadar.Shared.Spec;

namespace NavyRadar.Frontend.ViewModels;

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

    public ICommand RegisterCommand { get; }
    public ICommand HomeCommand => _navigationVm.HomeCommand;
    public ICommand SignInCommand => _navigationVm.SignInCommand;

    public RegisterVm(NavigationVm navigationVm)
    {
        _navigationVm = navigationVm;

        RegisterCommand = new SimpleRelayCommand(
            () => _ = ExecuteLoadingTask(OnRegisterAsync),
            () => !IsLoading);
    }

    private async Task OnRegisterAsync()
    {
        var registerDto = new PayloadRegister
        {
            Username = Username,
            Password = Password,
            Email = Email
        };
        var account = await ApiService.ApiClient.RegisterAsync(registerDto);
        ApiService.ApiClient.SetJwtToken(account.Token);

        _navigationVm.NavigateToMain(account.UserAccount.ToEntity().ToPasswordAccountEntity());
    }
}