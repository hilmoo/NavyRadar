namespace SharedModels.Models.AccountModel;

public class Account
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }

    protected Account() { }

    protected Account(Account account)
    {
        Id = account.Id;
        Username = account.Username;
        Password = account.Password;
        Email = account.Email;
        Role = account.Role;
    }

    public static void Register() { }
    public static void Login() { }
    public static void Logout() { }
}