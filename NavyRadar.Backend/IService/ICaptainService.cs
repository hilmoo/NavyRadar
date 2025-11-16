using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.IService;

public interface ICaptainService
{
    Task<IEnumerable<Captain>> GetAllAsync();
    Task<Captain?> GetByIdAsync(int id);
    Task<Captain?> CreateAsync(Captain captain);
    Task<Captain?> UpdateAsync(int id, Captain captain);
    Task<bool> DeleteAsync(int id);
}