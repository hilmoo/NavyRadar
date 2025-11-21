namespace NavyRadar.Shared.Domain.Auth;

public class AccountWithAuth
{
    public required string Token { get; set; }
    public required Entities.AccountBase UserAccount { get; set; }
}