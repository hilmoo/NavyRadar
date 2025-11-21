using NavyRadar.Shared.Domain.Account;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Backend.IService;

public interface IAccountService
{
    Task<IEnumerable<AccountBase>> GetAllAsync();
    Task<AccountBase?> GetByIdAsync(int id);
    Task<AccountBase?> CreateAsync(AccountPassword account);
    Task<AccountBase?> UpdateAsync(int id, UpdateAccount account);
    Task<bool> DeleteAsync(int id);
}