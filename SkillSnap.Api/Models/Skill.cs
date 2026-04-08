using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSnap.Api.Models;

public class Skill
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Level { get; set; } = string.Empty; // e.g. Beginner, Intermediate, Advanced

    // Foreign key
    [ForeignKey(nameof(PortfolioUser))]
    public int PortfolioUserId { get; set; }

    // Navigation
    public PortfolioUser? PortfolioUser { get; set; }
}
