using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Helpers.Jwt;
using WebApi.DataAccess.Abstract;
using Newtonsoft.Json;
using WebApi.Entities.Concrete.Dtos;
using WebApi.DataAccess.Context;
using WebApi.DataAccess.Redis;

namespace WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authService;
		private readonly JwtTokenGenerator _jwtTokenGenerator;
		private readonly DatabaseContext _context;
		private readonly IRedisCacheService _redisCacheService;

		public AuthController(IAuthRepository authService, JwtTokenGenerator jwtTokenGenerator, DatabaseContext context, IRedisCacheService redisCacheService)
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

			var accessToken = _jwtTokenGenerator.GenerateToken(user);
			var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

			var refreshTokenData = new RefreshToken
			{
				UserId = user.UserId,
				Token = refreshToken,
				ExpireTime = DateTime.Now.AddHours(1)
			};

			await _redisCacheService.SetValueAsync(
				$"refresh-token:{user.UserId}",
				JsonConvert.SerializeObject(refreshTokenData),
				TimeSpan.FromDays(7));

			return Ok(new
			{
				accessToken,
				refreshToken,
				user.UserId,
				user.Username,
				user.Email
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

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshToken model)
		{
			if (model == null || string.IsNullOrEmpty(model.Token))
				return BadRequest("Refresh token boş olamaz.");

			var key = $"refresh-token:{model.UserId}";
			var savedTokenJson = await _redisCacheService.GetValueAsync(key);

			if (string.IsNullOrEmpty(savedTokenJson))
				return Unauthorized("Kayıtlı refresh token bulunamadı.");

			var savedToken = JsonConvert.DeserializeObject<RefreshToken>(savedTokenJson);

			if (savedToken.Token != model.Token || savedToken.ExpireTime < DateTime.Now)
				return Unauthorized("Refresh token geçersiz veya süresi dolmuş.");

			// Kullanıcıyı veritabanından çek
			var user = await _authService.GetUserByIdAsync(model.UserId);
			if (user == null)
				return Unauthorized("Kullanıcı bulunamadı.");

			// Yeni access ve refresh token üret
			var newAccessToken = _jwtTokenGenerator.GenerateToken(user);
			var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

			var newTokenData = new RefreshToken
			{
				UserId = user.UserId,
				Token = newRefreshToken,
				ExpireTime = DateTime.Now.AddDays(7)
			};

			await _redisCacheService.SetValueAsync(
				key,
				JsonConvert.SerializeObject(newTokenData),
				TimeSpan.FromDays(7));

			return Ok(new
			{
				accessToken = newAccessToken,
				refreshToken = newRefreshToken
			});
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
			await _redisCacheService.Clear($"refresh-token:{userId}");
			return Ok("Çıkış yapıldı ve tokenlar temizlendi.");
		}

		[HttpGet("getuserwithroles")]
		public async Task<IActionResult> GetUsersByRole(string roleName)
		{
			var result = await _context.Set<UserDto>()
				.FromSqlInterpolated($"EXEC GetUsersWithRoleName @RoleName={roleName}")
				.ToListAsync();

			return Ok(result);
		}


	}
}
