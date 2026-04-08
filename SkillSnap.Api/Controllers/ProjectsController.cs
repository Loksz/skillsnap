using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;
using System.Diagnostics;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly SkillSnapContext _context;
    private readonly IMemoryCache     _cache;
    private const string CacheKey = "projects_all";

    public ProjectsController(SkillSnapContext context, IMemoryCache cache)
    {
        _context = context;
        _cache   = cache;
    }

    // GET /api/projects  — cached, public
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sw = Stopwatch.StartNew();

        if (_cache.TryGetValue(CacheKey, out List<Project>? cached))
        {
            sw.Stop();
            Response.Headers.Append("X-Cache", "HIT");
            Response.Headers.Append("X-Duration-Ms", sw.ElapsedMilliseconds.ToString());
            return Ok(cached);
        }

        // Cache miss — query DB with AsNoTracking (read-only optimization)
        var projects = await _context.Projects
            .AsNoTracking()
            .Include(p => p.PortfolioUser)
            .ToListAsync();

        _cache.Set(CacheKey, projects, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration               = TimeSpan.FromMinutes(2)
        });

        sw.Stop();
        Response.Headers.Append("X-Cache", "MISS");
        Response.Headers.Append("X-Duration-Ms", sw.ElapsedMilliseconds.ToString());
        return Ok(projects);
    }

    // GET /api/projects/{id} — public
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await _context.Projects
            .AsNoTracking()
            .Include(p => p.PortfolioUser)
            .FirstOrDefaultAsync(p => p.Id == id);

        return project is null ? NotFound() : Ok(project);
    }

    // POST /api/projects — requires authentication (Admin role)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Project project)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Invalidate cache after write
        _cache.Remove(CacheKey);

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    // DELETE /api/projects/{id} — requires Admin role
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project is null) return NotFound();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);

        return NoContent();
    }
}
