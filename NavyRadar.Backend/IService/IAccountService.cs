using NavyRadar.Shared.Domain.Account;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.IService;

public interface IAccountService
{
    Task<IEnumerable<Account>> GetAllAsync();
    Task<Account?> GetByIdAsync(int id);
    Task<Account?> CreateAsync(Account account);
    Task<Account?> UpdateAsync(int id, UpdateAccount account);
    Task<bool> DeleteAsync(int id);
}