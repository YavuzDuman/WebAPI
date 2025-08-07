using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers.Hashing;
using WebApi.DataAccess.Abstract;
using WebApi.Entities.Concrete;
using WebApi.DataAccess.Context;

namespace WebApi.DataAccess.Concrete
{
	public class UserRepository : EfRepository<User>, IUserRepository
	{
		public UserRepository(DatabaseContext context) : base(context) { }

		public List<User> GetAllWithRoles()
		{
			var users = _context.Users
				.Include(u => u.UserRoles)
				.ThenInclude(ur => ur.Role).ToList();
			return users;
		}

		public User GetByIdWithRoles(int id)
		{
			var user = _context.Users
				.Include(u => u.UserRoles)
				.ThenInclude(ur => ur.Role)
				.FirstOrDefault(u => u.UserId == id );
			return user;
		}
	}
}
