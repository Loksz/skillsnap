using System.Net.Http.Json;

namespace SkillSnap.Client.Services;

public class SkillDto
{
    public int    Id              { get; set; }
    public string Name           { get; set; } = string.Empty;
    public string Level          { get; set; } = string.Empty;
    public int    PortfolioUserId { get; set; }
}

public class SkillService
{
    private readonly HttpClient _http;

    public SkillService(HttpClient http) => _http = http;

    public async Task<List<SkillDto>> GetSkillsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<SkillDto>>("api/skills")
                   ?? new List<SkillDto>();
        }
        catch
        {
            return new List<SkillDto>();
        }
    }

    public async Task<bool> AddSkillAsync(SkillDto skill)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/skills", skill);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
