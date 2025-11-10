namespace NavyRadar.Frontend.Util;

using Shared.Spec;
using System.Net.Http;

public static class ApiService
{
    private static readonly HttpClient HttpClient = new();

    public static Client ApiClient { get; private set; }

    static ApiService()
    {
        ApiClient = new Client(HttpClient);
    }
}