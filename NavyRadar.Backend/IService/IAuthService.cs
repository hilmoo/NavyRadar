using NavyRadar.Shared.Domain;

namespace NavyRadar.Backend.IService;

public interface IAuthService
{
    Task<AccWithAuth?> RegisterAsync(RegisterDto registerDto);
    Task<AccWithAuth?> SignInService(LoginDto loginDto);
}