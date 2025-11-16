namespace NavyRadar.Shared.Domain.Auth;

public class PayloadLogin
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}