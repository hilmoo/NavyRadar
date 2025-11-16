using System.Windows.Input;
using NavyRadar.Frontend.Util;
using NavyRadar.Shared.Entities;
using PayloadLogin = NavyRadar.Shared.Spec.PayloadLogin;

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

    public ICommand SignInCommand { get; }
    public ICommand HomeCommand => _navigationVm.HomeCommand;
    public ICommand RegisterCommand => _navigationVm.RegisterCommand;

    public SignInVm(NavigationVm navigationVm)
    {
        _navigationVm = navigationVm;

        SignInCommand = new SimpleRelayCommand(
            void () => _ = ExecuteLoadingTask(OnLoginAsync),
            () => !IsLoading);
    }

    private async Task OnLoginAsync()
    {
        var loginDto = new PayloadLogin
        {
            Username = Username,
            Password = Password
        };
        var account = await ApiService.ApiClient.SigninAsync(loginDto);
        ApiService.ApiClient.SetJwtToken(account.Token);

        _navigationVm.NavigateToMain(account.UserAccount.ToEntity());
    }
}