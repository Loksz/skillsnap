using System.Net.Http.Json;

namespace SkillSnap.Client.Services;

public class ProjectDto
{
    public int    Id              { get; set; }
    public string Title          { get; set; } = string.Empty;
    public string Description    { get; set; } = string.Empty;
    public string ImageUrl       { get; set; } = string.Empty;
    public int    PortfolioUserId { get; set; }
}

public class ProjectService
{
    private readonly HttpClient _http;

    public ProjectService(HttpClient http) => _http = http;

    public async Task<List<ProjectDto>> GetProjectsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<ProjectDto>>("api/projects")
                   ?? new List<ProjectDto>();
        }
        catch
        {
            return new List<ProjectDto>();
        }
    }

    public async Task<bool> AddProjectAsync(ProjectDto project)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/projects", project);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteProjectAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/projects/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
