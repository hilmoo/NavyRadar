using NavyRadar.Shared.Domain.Sail;
using NavyRadar.Shared.Domain.Sailing;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.IService;

public interface ISailingService
{
    Task<SailWithName?> GetActiveSailByCaptainIdAsync(int captainId);
    Task<bool> AddPositionHistoryAsync(int accountId, AddPositionRequest request);
    Task<bool> CompleteActiveSailAsync(int accountId);
    Task<bool> UpdateSailStatusAsync(int accountId, SailStatus newStatus);
}