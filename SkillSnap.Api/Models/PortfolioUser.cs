using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Api.Models;

public class PortfolioUser
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Bio { get; set; } = string.Empty;

    public string ProfileImageUrl { get; set; } = string.Empty;

    // Navigation properties
    public List<Project> Projects { get; set; } = new();
    public List<Skill>   Skills   { get; set; } = new();
}
