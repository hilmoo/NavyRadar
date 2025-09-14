using SharedModels.Contracts.Dtos;

namespace Backend.Service;

public class AccountService
{
    public Account? Register(string username, string password, string email, bool isAdmin = false)
    {
        // TODO: 
        return null;
    }

    public Account? Login(string username, string password)
    {
        // TODO: 
        return null;
    }
    
    public Account? GetUser(string id)
    {
        // TODO: 
        return null;
    }
    
    public List<Account>? ListAllUser()
    {
        // TODO: 
        return null;
    }
    
    public Account? UpdateAccount(string id, Account account)
    {
        // TODO: 
        return null;
    }

    public Account? RemoveUser(string id)
    {
        // TODO: 
        return null;
    }
}