using Shared.Domain;
using Shared.Models;

namespace Backend.IService;

public interface ISailService
{
    Task<IEnumerable<Sail>> GetAllAsync();
    Task<IEnumerable<ActiveSailPosition>> GetAllActiveSailPositionAsync();
    Task<Sail?> GetByIdAsync(int id);
    Task<Sail?> CreateAsync(Sail sail);
    Task<Sail?> UpdateAsync(int id, Sail sail);
    Task<bool> DeleteAsync(int id);
}