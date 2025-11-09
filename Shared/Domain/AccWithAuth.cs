using Shared.Models;

namespace Shared.Domain;

public class AccWithAuth
{
    public required string Token { get; set; }
    public required Account UserAccount { get; set; }
}