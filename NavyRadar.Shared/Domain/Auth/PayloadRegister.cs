namespace NavyRadar.Shared.Domain.Auth;

public class PayloadRegister
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}