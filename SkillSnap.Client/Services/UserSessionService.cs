namespace SkillSnap.Client.Services;

/// <summary>
/// Scoped service that persists authenticated user state across components
/// without needing to re-fetch from the API on every navigation.
/// </summary>
public class UserSessionService
{
    public string? UserId    { get; private set; }
    public string? Email     { get; private set; }
    public string? Role      { get; private set; }
    public bool    IsLoggedIn => !string.IsNullOrEmpty(Token);
    public string? Token     { get; private set; }

    // Currently selected project for editing state
    public int? ActiveProjectId { get; private set; }

    public event Action? OnChange;

    public void SetSession(string token, string email, string role)
    {
        Token  = token;
        Email  = email;
        Role   = role;
        NotifyStateChanged();
    }

    public void SetActiveProject(int? projectId)
    {
        ActiveProjectId = projectId;
        NotifyStateChanged();
    }

    public void ClearSession()
    {
        Token           = null;
        Email           = null;
        Role            = null;
        UserId          = null;
        ActiveProjectId = null;
        NotifyStateChanged();
    }

    public bool IsAdmin => Role == "Admin";

    private void NotifyStateChanged() => OnChange?.Invoke();
}
