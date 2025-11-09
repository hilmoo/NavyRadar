namespace Shared.Spec;

public partial class Client
{
    private string? _jwtToken;

    public void SetJwtToken(string token)
    {
        _jwtToken = token;
    }

    public void ClearJwtToken()
    {
        _jwtToken = null;
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
    {
        if (!string.IsNullOrEmpty(_jwtToken))
        {
            request.Headers.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
        }
    }
}