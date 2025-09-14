namespace SharedModels.Contracts.Dtos;

public class AccountCaptain: Account
{
    public List<string> AssignedShipIds { get; set; } = [];
    public string? CurrentShipId { get; set; }
}