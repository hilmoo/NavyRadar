using NavyRadar.Shared.Models;

namespace NavyRadar.Shared.Domain;

public class AccWithAuth
{
    public required string Token { get; set; }
    public required Account UserAccount { get; set; }
}