namespace SharedModels.Models.AccountModel;

public abstract class CaptainService
{
    private readonly Dictionary<string, Captain> _captains = new();

    public Captain? GetCaptainById(string captainId)
    {
        return _captains.GetValueOrDefault(captainId);
    }

    public List<Captain> GetAllCaptains()
    {
        return _captains.Values.ToList();
    }

    public void AddCaptain(Captain captain)
    {
        _captains[captain.Id] = captain;
    }

    public void RemoveCaptain(string captainId)
    {
        _captains.Remove(captainId);
    }

    public void UpdateCaptain(Captain updatedCaptain)
    {
        if (_captains.ContainsKey(updatedCaptain.Id))
        {
            _captains[updatedCaptain.Id] = updatedCaptain;
        }
    }

    public void AssignShipToCaptain(string captainId, string shipId)
    {
        if (_captains.TryGetValue(captainId, out Captain? captain) && !captain.AssignedShipIds.Contains(shipId))
        {
            captain.AssignedShipIds.Add(shipId);
        }
    }
}
