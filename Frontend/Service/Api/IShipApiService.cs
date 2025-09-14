using SharedModels.Contracts.Dtos;

namespace Frontend.Service.Api;

public interface IShipApiService
{
    Task<Ship?>? GetShipById(string id);
}