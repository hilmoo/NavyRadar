using NavyRadar.Shared.Models;

namespace NavyRadar.Backend.IService;

public interface IPositionHistoryService
{
    Task<IEnumerable<PositionHistory>> GetAllAsync();
    Task<PositionHistory?> GetByIdAsync(int id);
    Task<IEnumerable<PositionHistory>> GetBySailIdAsync(int sailId);
    Task<PositionHistory?> CreateAsync(PositionHistory positionHistory);
    Task<PositionHistory?> UpdateAsync(int id, PositionHistory positionHistory);
    Task<bool> DeleteAsync(int id);
}