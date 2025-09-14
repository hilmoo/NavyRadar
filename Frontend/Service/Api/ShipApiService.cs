using System.Net.Http;
using System.Text.Json;
using SharedModels.Contracts.Dtos;

namespace Frontend.Service.Api;

public class ShipApiService : IShipApiService
{
    private static readonly HttpClient Client = new();
    private const string BaseUrl = "http://localhost:3000";
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public async Task<Ship?>? GetShipById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        var url = $"{BaseUrl}/ships/{id}";

        try
        {
            var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            var ship = JsonSerializer.Deserialize<Ship>(responseBody, _jsonOptions);
            return ship;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An unexpected error occurred for ship '{id}': {e.Message}");
            return null;
        }
    }
}