using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser>  _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration                _config;

    public AuthController(
        UserManager<ApplicationUser>  userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration                config)
    {
        _userManager  = userManager;
        _signInManager = signInManager;
        _config        = config;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            UserName = req.Email,
            Email    = req.Email,
            FullName = req.FullName
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        // Assign default role
        await _userManager.AddToRoleAsync(user, "User");

        return Ok(new { message = "Registration successful." });
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user == null) return Unauthorized("Invalid credentials.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!result.Succeeded) return Unauthorized("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwt(user, roles);

        return Ok(new { token, email = user.Email, roles });
    }

    private string GenerateJwt(ApplicationUser user, IList<string> roles)
    {
        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiresInMinutes"]!));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer:             _config["Jwt:Issuer"],
            audience:           _config["Jwt:Audience"],
            claims:             claims,
            expires:            expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record RegisterRequest(
    [property: System.ComponentModel.DataAnnotations.Required] string FullName,
    [property: System.ComponentModel.DataAnnotations.Required,
               System.ComponentModel.DataAnnotations.EmailAddress] string Email,
    [property: System.ComponentModel.DataAnnotations.Required,
               System.ComponentModel.DataAnnotations.MinLength(6)] string Password);

public record LoginRequest(
    [property: System.ComponentModel.DataAnnotations.Required,
               System.ComponentModel.DataAnnotations.EmailAddress] string Email,
    [property: System.ComponentModel.DataAnnotations.Required] string Password);
