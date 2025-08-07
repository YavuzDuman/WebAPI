using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers.Hashing;
using WebApi.DataAccess.Abstract;
using WebApi.Entities.Concrete;
using WebApi.Entities.Concrete.Dtos;
using WebApi.DataAccess.Context;

namespace WebApi.DataAccess.Concrete
{
	public class AuthRepository : IAuthRepository
	{
		private readonly DatabaseContext _context;
		private readonly IMapper _mapper;

		public AuthRepository(DatabaseContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public User LoginUser(LoginDto loginUser)
		{
			var user = _context.Users
				.Include(u => u.UserRoles)
				.ThenInclude(ur => ur.Role)
				.FirstOrDefault(u => u.Username == loginUser.Username && u.Password == PasswordHasher.HashPassword(loginUser.Password));
			if (user == null) return null;
		
			return user;
		}

		public void RegisterUser(RegisterDto dto)
		{
			var user = _mapper.Map<User>(dto);
			user.Password = PasswordHasher.HashPassword(user.Password);
			user.InsertDate = DateTime.Now;
			user.IsActive = true;
			_context.Users.Add(user);
			_context.SaveChanges();
		}

		public async Task<User> GetUserByIdAsync(int userId)
		{
			return await _context.Users
				.Include(u => u.UserRoles)
				.ThenInclude(ur => ur.Role)
				.FirstOrDefaultAsync(u => u.UserId == userId);
		}
	}
}
