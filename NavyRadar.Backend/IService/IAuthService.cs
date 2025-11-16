using NavyRadar.Shared.Domain.Auth;

namespace NavyRadar.Backend.IService;

public interface IAuthService
{
    Task<AccountWithAuth?> RegisterAsync(PayloadRegister payloadRegister);
    Task<AccountWithAuth?> SignInService(PayloadLogin payloadLogin);
}