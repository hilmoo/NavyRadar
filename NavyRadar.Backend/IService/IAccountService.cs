using NavyRadar.Shared.Domain;
using NavyRadar.Shared.Models;

namespace NavyRadar.Backend.IService;

public interface IAccountService
{
    Task<IEnumerable<Account>> GetAllAsync();
    Task<Account?> GetByIdAsync(int id);
    Task<Account?> CreateAsync(Account account);
    Task<Account?> UpdateAsync(int id, UpdateAcc account);
    Task<bool> DeleteAsync(int id);
}