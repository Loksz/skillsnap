using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly SkillSnapContext _context;
    private readonly IMemoryCache     _cache;
    private const string CacheKey = "skills_all";

    public SkillsController(SkillSnapContext context, IMemoryCache cache)
    {
        _context = context;
        _cache   = cache;
    }

    // GET /api/skills — cached, public
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (_cache.TryGetValue(CacheKey, out List<Skill>? cached))
        {
            Response.Headers.Append("X-Cache", "HIT");
            return Ok(cached);
        }

        var skills = await _context.Skills
            .AsNoTracking()
            .Include(s => s.PortfolioUser)
            .ToListAsync();

        _cache.Set(CacheKey, skills, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration               = TimeSpan.FromMinutes(2)
        });

        Response.Headers.Append("X-Cache", "MISS");
        return Ok(skills);
    }

    // GET /api/skills/{id} — public
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var skill = await _context.Skills
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        return skill is null ? NotFound() : Ok(skill);
    }

    // POST /api/skills — requires authentication
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] Skill skill)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);

        return CreatedAtAction(nameof(GetById), new { id = skill.Id }, skill);
    }

    // DELETE /api/skills/{id} — requires Admin role
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill is null) return NotFound();

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();
        _cache.Remove(CacheKey);

        return NoContent();
    }
}
