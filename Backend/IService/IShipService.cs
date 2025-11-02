using Shared.Models;

namespace Backend.IService;

public interface IShipService
{
    Task<IEnumerable<Ship>> GetAllAsync();
    Task<Ship?> GetByIdAsync(int id);
    Task<Ship?> CreateAsync(Ship ship);
    Task<Ship?> UpdateAsync(int id, Ship ship);
    Task<bool> DeleteAsync(int id);
}