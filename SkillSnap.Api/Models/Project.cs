using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSnap.Api.Models;

public class Project
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    // Foreign key
    [ForeignKey(nameof(PortfolioUser))]
    public int PortfolioUserId { get; set; }

    // Navigation
    public PortfolioUser? PortfolioUser { get; set; }
}
