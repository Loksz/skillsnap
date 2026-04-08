using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services;

public class AuthService
{
    private readonly HttpClient        _http;
    private readonly IJSRuntime        _js;
    private readonly UserSessionService _session;

    public AuthService(HttpClient http, IJSRuntime js, UserSessionService session)
    {
        _http    = http;
        _js      = js;
        _session = session;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new { email, password });
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result?.Token is null) return false;

            // Persist token in localStorage
            await _js.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);

            var role = result.Roles?.FirstOrDefault() ?? "User";
            _session.SetSession(result.Token, result.Email ?? email, role);

            // Attach token to future requests
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RegisterAsync(string fullName, string email, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", new { fullName, email, password });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        _http.DefaultRequestHeaders.Authorization = null;
        _session.ClearSession();
    }

    public async Task TryRestoreSessionAsync()
    {
        var token = await _js.InvokeAsync<string?>("localStorage.getItem", "authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            // Simplified restore — in production decode JWT claims
            _session.SetSession(token, "", "User");
        }
    }

    private record LoginResponse(string? Token, string? Email, List<string>? Roles);
}
