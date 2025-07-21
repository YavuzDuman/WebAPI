using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Entities.Dtos;
using WebApi.Helpers.Hashing;
using WebApi.Helpers.Mapping;
using WebApi.Services.Abstract;

namespace WebApi.Services.Concrete
{
	public class AuthService : IAuthService
	{
		private readonly DatabaseContext _context;
		private readonly IMapper _mapper;

		public AuthService(DatabaseContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public User LoginUser(LoginDto loginUser)
		{
			var user = _context.Users
				.Include(u => u.Role)
				.FirstOrDefault(u => u.Username == loginUser.Username && u.Password == PasswordHasher.HashPassword(loginUser.Password));
			if (user == null) return null;
		
			return user;
		}

		public void RegisterUser(RegisterDto dto)
		{
			var user = _mapper.Map<User>(dto);

			_context.Users.Add(user);
			_context.SaveChanges();
		}
	}
}
