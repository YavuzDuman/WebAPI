using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Entities;
using WebApi.Entities.Dtos;
using WebApi.Helpers.Jwt;
using WebApi.DataAccess.Abstract;
using WebApi.DataAccess.Concrete;
using Newtonsoft.Json;

namespace WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
    {
        private readonly IAuthDal _authService;
		private readonly JwtTokenGenerator _jwtTokenGenerator;
		private readonly DatabaseContext _context;
		private readonly IRedisCacheService _redisCacheService;

		public AuthController(IAuthDal authService, JwtTokenGenerator jwtTokenGenerator, DatabaseContext context, IRedisCacheService redisCacheService)
		{
			_authService = authService;
			_jwtTokenGenerator = jwtTokenGenerator;
			_context = context;
			_redisCacheService = redisCacheService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto loginUser)
		{
			var user = _authService.LoginUser(loginUser);
			if (user == null)
				return Unauthorized("Kullanıcı adı veya şifre yanlış.");

			var token = _jwtTokenGenerator.GenerateToken(user);

			var session = new LoggedUser
			{
				UserId = user.UserId,
				Username = user.Username,
				LoginTime = DateTime.Now,
				ExpireTime = DateTime.Now.AddMinutes(1)
			};

			var success = await _redisCacheService.SetValueAsync(
				$"logged-user:{user.UserId}",
				JsonConvert.SerializeObject(session));

			if (!success)
			{
			Console.WriteLine("redise yazılamadı.");
			}

			return Ok(new
			{
				user.UserId,
				user.Name,
				user.Username,
				user.Email,
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

				var claims = jwtToken.Claims
					.GroupBy(c => c.Type)
					.ToDictionary(
						g => g.Key,
						g => string.Join(",", g.Select(c => c.Value))
					);

				return Ok(new
				{
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
			var users = await _context.Set<UserDto>().FromSqlRaw("EXEC GetUsersWithRoles").ToListAsync();

			return Ok(users);
		}

		[HttpGet("active-user/{userId}")]
		public async Task<IActionResult> GetLoggedUser(int userId)
		{
			var json = await _redisCacheService.GetValueAsync($"logged-user:{userId}");
			if (json == null)
				return NotFound("Oturum bulunamadı.");

			var session = JsonConvert.DeserializeObject<LoggedUser>(json);
			return Ok(session);
		}

		[HttpPost("logout/{userId}")]
		public async Task<IActionResult> Logout(int userId)
		{
			await _redisCacheService.Clear($"logged-user:{userId}");
			return Ok("çıkış yapıldı");
		}


	}
}
