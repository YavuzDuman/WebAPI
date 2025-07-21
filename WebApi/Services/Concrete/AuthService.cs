using Microsoft.AspNetCore.Identity;
using WebApi.Entities;
using WebApi.Entities.Dtos;
using WebApi.Helpers;
using WebApi.Services.Abstract;

namespace WebApi.Services.Concrete
{
	public class AuthService : IAuthService
	{
		private readonly DatabaseContext _context;

		public AuthService(DatabaseContext context)
		{
			_context = context;
		}

		public User LoginUser(LoginDto loginUser)
		{
			var user = _context.Users.FirstOrDefault(u => u.Username == loginUser.Username);
			if (user == null) return null;
			string hashedInputPassword = PasswordHasher.HashPassword(loginUser.Password);
			if (user.Password == hashedInputPassword)
			{
				return user;
			}
			else
			{
				return null;
			}
		}

		public void RegisterUser(RegisterDto dto)
		{
			var user = new User
			{
				Name = dto.Name,
				Username = dto.Username,
				Email = dto.Email,
				Password = PasswordHasher.HashPassword(dto.Password),
				InsertDate = DateTime.Now,
				IsActive = true
			};

			_context.Users.Add(user);
			_context.SaveChanges();
		}
	}
}
