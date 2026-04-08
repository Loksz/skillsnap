using Microsoft.AspNetCore.Mvc;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly SkillSnapContext _context;

    public SeedController(SkillSnapContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Seed()
    {
        if (_context.PortfolioUsers.Any())
            return Ok("Seed data already exists.");

        var user = new PortfolioUser
        {
            Name            = "Alex Rivera",
            Bio             = "Full-stack developer passionate about clean code and great UX.",
            ProfileImageUrl = "https://i.pravatar.cc/150?img=12",
            Projects = new List<Project>
            {
                new() { Title = "SkillSnap",       Description = "Full-stack portfolio tracker built with Blazor & ASP.NET Core.", ImageUrl = "https://picsum.photos/seed/skillsnap/400/200" },
                new() { Title = "WarehouseX API",  Description = "Order management REST API with JWT auth and EF Core.",          ImageUrl = "https://picsum.photos/seed/warehouse/400/200"  },
                new() { Title = "DSA Visualizer",  Description = "Interactive data structures demo using C# and Blazor.",        ImageUrl = "https://picsum.photos/seed/dsa/400/200"        }
            },
            Skills = new List<Skill>
            {
                new() { Name = "C#",           Level = "Advanced"     },
                new() { Name = "ASP.NET Core", Level = "Advanced"     },
                new() { Name = "Blazor",       Level = "Intermediate" },
                new() { Name = "Entity Framework Core", Level = "Intermediate" },
                new() { Name = "SQL / SQLite", Level = "Intermediate" },
                new() { Name = "HTML & CSS",   Level = "Advanced"     }
            }
        };

        _context.PortfolioUsers.Add(user);
        _context.SaveChanges();

        return Ok("Seed data inserted successfully.");
    }
}
