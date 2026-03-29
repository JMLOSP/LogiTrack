using LogiTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LogiTrack.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
      ApplicationUser user = new ApplicationUser
      {
        UserName = request.Email,
        Email = request.Email
      };

      IdentityResult result = await _userManager.CreateAsync(user, request.Password);

      if (!result.Succeeded)
        return BadRequest(result.Errors);

      return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
      ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);

      if (user == null)
        return Unauthorized();

      Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

      if (!result.Succeeded)
        return Unauthorized();

      string token = await GenerateJwtToken(user);

      return Ok(new { token });
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
      string jwtKey = _configuration["Jwt:Key"]!;
      string jwtIssuer = _configuration["Jwt:Issuer"]!;
      string jwtAudience = _configuration["Jwt:Audience"]!;
      int expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!);

      SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
      SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      IList<string> roles = await _userManager.GetRolesAsync(user);

      List<Claim> claims = new List<Claim>
      {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
      };

      foreach (string role in roles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }

      JwtSecurityToken token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
        signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(string email, string role)
    {
      var user = await _userManager.FindByEmailAsync(email);

      if (user == null)
        return NotFound();

      await _userManager.AddToRoleAsync(user, role);

      return Ok();
    }
  }
}