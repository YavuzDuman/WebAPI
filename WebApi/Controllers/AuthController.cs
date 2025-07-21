using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities.Dtos;
using WebApi.Helpers.Jwt;
using WebApi.Services.Abstract;

namespace WebApi.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
		private readonly JwtTokenGenerator _jwtTokenGenerator;

		public AuthController(IAuthService authService, JwtTokenGenerator jwtTokenGenerator)
		{
			_authService = authService;
			_jwtTokenGenerator = jwtTokenGenerator;
		}

		[AllowAnonymous]
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
	}
}
