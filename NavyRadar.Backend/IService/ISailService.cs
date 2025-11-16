using NavyRadar.Shared.Domain.Sail;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.IService;

public interface ISailService
{
    Task<SailWithName?> CreateAsync(Sail sail);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SailWithName>> GetAllAsync();
    Task<IEnumerable<ActiveSailPosition>> GetAllActiveSailPositionAsync();
    Task<SailWithName?> GetByIdAsync(int id);
    Task<SailWithName?> UpdateAsync(int id, Sail sail);
}