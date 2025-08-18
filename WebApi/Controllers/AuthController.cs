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
using WebApi.Business.Abstract;
using WebApi.Helpers.Validator;
using FluentValidation;

namespace WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthManager _authManager;
		private readonly JwtTokenGenerator _jwtTokenGenerator;
		private readonly IRedisCacheService _redisCacheService;


		public AuthController(IAuthManager authManager, JwtTokenGenerator jwtTokenGenerator, IRedisCacheService redisCacheService)
		{
			_authManager = authManager;
			_jwtTokenGenerator = jwtTokenGenerator;
			_redisCacheService = redisCacheService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterDto registerUser, CancellationToken ct)
		{
			var ok = await _authManager.RegisterUserAsync(registerUser, ct);
			if (!ok) return BadRequest("Bu kullanıcı adı veya e-posta zaten kayıtlı.");

			return Ok("Kullanıcı başarıyla kaydedildi.");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto loginUser, CancellationToken ct)
		{

			var user = await _authManager.LoginUserAsync(loginUser, ct);
			if (user == null)
			{
				return Unauthorized("Kullanıcı adı veya şifre yanlış.");
			}

			var accessToken = _jwtTokenGenerator.GenerateToken(user);

			var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
			var refreshTtlDays = 14;
			var expireAt = DateTime.UtcNow.AddDays(refreshTtlDays);

			var data = new RefreshToken
			{
				UserId = user.UserId,
				Token = refreshToken,
				ExpireTime = expireAt
			};

			// Redis Key Yapısı Güncellendi
			var key = $"auth:refreshtoken:{user.UserId}";
			await _redisCacheService.SetValueAsync(key,
				JsonConvert.SerializeObject(data),
				TimeSpan.FromDays(refreshTtlDays));

			return Ok(new { accessToken, refreshToken, user.UserId, user.Username, user.Email });
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshToken model, CancellationToken ct)
		{
			if (model == null || string.IsNullOrEmpty(model.Token))
				return BadRequest("Refresh token boş olamaz.");

			// Redis Key Yapısı Güncellendi
			var key = $"auth:refreshtoken:{model.UserId}";
			var savedTokenJson = await _redisCacheService.GetValueAsync(key);
			if (string.IsNullOrEmpty(savedTokenJson))
				return Unauthorized("Kayıtlı refresh token bulunamadı.");

			var saved = JsonConvert.DeserializeObject<RefreshToken>(savedTokenJson);
			if (saved == null || saved.Token != model.Token || saved.ExpireTime <= DateTime.UtcNow)
				return Unauthorized("Refresh token geçersiz veya süresi dolmuş.");

			var user = await _authManager.GetUserByIdAsync(model.UserId, ct);
			if (user == null) return Unauthorized("Kullanıcı bulunamadı.");

			var newAccess = _jwtTokenGenerator.GenerateToken(user);
			var newRefresh = _jwtTokenGenerator.GenerateRefreshToken();

			var days = 14;
			var expireAt = DateTime.UtcNow.AddDays(days);

			var newData = new RefreshToken { UserId = user.UserId, Token = newRefresh, ExpireTime = expireAt };

			await _redisCacheService.SetValueAsync(key,
				JsonConvert.SerializeObject(newData),
				TimeSpan.FromDays(days));

			return Ok(new { accessToken = newAccess, refreshToken = newRefresh });
		}


		[Authorize(Roles = "admin")]
		[HttpGet("decode")]
		public IActionResult DecodeToken(string token)
		{
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



		[HttpGet("active-user/{userId}")]
		public async Task<IActionResult> GetLoggedUser(int userId)
		{
			// Redis Key Yapısı Güncellendi
			var json = await _redisCacheService.GetValueAsync($"auth:loggeduser:{userId}");
			if (json == null)
				return NotFound("Oturum bulunamadı.");

			var session = JsonConvert.DeserializeObject<LoggedUser>(json);
			return Ok(session);
		}

		[HttpPost("logout/{userId}")]
		public async Task<IActionResult> Logout(int userId)
		{
			// Redis Key Yapısı Güncellendi
			await _redisCacheService.Clear($"auth:loggeduser:{userId}");
			await _redisCacheService.Clear($"auth:refreshtoken:{userId}");
			return Ok("Çıkış yapıldı ve tokenlar temizlendi.");
		}

	}
}
