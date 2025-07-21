using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Entities;
using WebApi.Entities.Dtos;
using WebApi.Helpers.Jwt;
using WebApi.Services.Abstract;

namespace WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
		private readonly JwtTokenGenerator _jwtTokenGenerator;
		private readonly DatabaseContext _context;

		public AuthController(IAuthService authService, JwtTokenGenerator jwtTokenGenerator, DatabaseContext context)
		{
			_authService = authService;
			_jwtTokenGenerator = jwtTokenGenerator;
			_context = context;
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody] LoginDto loginUser)
		{
			var user = _authService.LoginUser(loginUser);
			if (user == null)
				return Unauthorized("Kullanıcı adı veya şifre yanlış.");

			var token = _jwtTokenGenerator.GenerateToken(user);

			return Ok(new
			{
				user.UserId,
				user.Name,
				user.Username,
				user.Email,
				user.RoleId,
				Token = token
			});
		}


		[HttpPost("register")]
		public IActionResult Register(RegisterDto registerUser)
		{
			if (registerUser == null || string.IsNullOrEmpty(registerUser.Username) || string.IsNullOrEmpty(registerUser.Password))
				return BadRequest("Kullanıcı adı ve şifre boş olamaz.");

			_authService.RegisterUser(registerUser);
			return Ok("Kullanıcı başarıyla kaydedildi.");
		}

		[Authorize(Roles = "admin")]
		[HttpGet("decode")]
		public IActionResult DecodeToken(string token) {
			try
			{
				var handler = new JwtSecurityTokenHandler();
				var jwtToken = handler.ReadJwtToken(token);

				var claims = jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);

				return Ok(new
				{
					Header = jwtToken.Header,
					Payload = claims,
					Expiration = jwtToken.ValidTo
				});
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = "Token geçersiz veya çözülemedi.", detay = ex.Message });
			}
		}

		[Authorize(Roles = "admin")]
		[HttpGet("with-roles")]
		public async Task<IActionResult> GetUsersWithRoles()
		{
			var users = await _context.UsersWithRolesDto
				.FromSqlRaw("EXEC GetUsersWithRoles")
				.ToListAsync();

			return Ok(users);
		}




	}
}
