using Shared.Domain;
using Shared.Models;

namespace Backend.IService;

public interface IAuthService
{
    Task<AccWithAuth?> RegisterAsync(RegisterDto registerDto);
    Task<AccWithAuth?> SignInService(LoginDto loginDto);
}