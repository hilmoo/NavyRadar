using Shared.Models;

namespace Backend.IService;

public interface IPortService
{
    Task<IEnumerable<Port>> GetAllAsync();
    Task<Port?> GetByIdAsync(int id);
    Task<Port?> CreateAsync(Port port);
    Task<Port?> UpdateAsync(int id, Port port);
    Task<bool> DeleteAsync(int id);
}