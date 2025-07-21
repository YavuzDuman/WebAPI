using WebApi.Entities;
using WebApi.Entities.Dtos;
using WebApi.Helpers;
using WebApi.Services.Abstract;

namespace WebApi.Services.Concrete
{
	public class UserService : IUserService
	{
		private readonly DatabaseContext _context;

		public UserService(DatabaseContext context)
		{
			_context = context;
		}
		public List<User> GetAllUsers()
		{
			return _context.Users.Where(u=>u.IsActive==true).ToList();
		}

		public void CreateUser(User user)
		{
			user.Password = PasswordHasher.HashPassword(user.Password);
			user.InsertDate = DateTime.Now;
			user.IsActive = true; 
			_context.Users.Add(user);
			_context.SaveChanges();
		}

		public void DeleteUser(int id)
		{
			var user = _context.Users.FirstOrDefault(u => u.UserId == id);
			if (user == null) return;

			_context.Users.Remove(user);
			_context.SaveChanges();
		}

		public User GetUserById(int id)
		{
			return _context.Users.FirstOrDefault(u => u.UserId == id && u.IsActive);
		}

		public void UpdateUser(int id, User updatedUser)
		{
			var user = _context.Users.FirstOrDefault(u => u.UserId == id);
			if (user == null) return;

			user.Name = updatedUser.Name;
			user.Username = updatedUser.Username;
			user.Email = updatedUser.Email;
			user.Password = PasswordHasher.HashPassword(updatedUser.Password);

			_context.SaveChanges();
		}

		public void SoftDeleteUserById(int id)
		{
			var user = _context.Users.FirstOrDefault(u => u.UserId == id);
			if (user == null) return;
			user.IsActive = false;
			_context.SaveChanges();
		}

		public List<User> GetAllUsersOrderByDate()
		{
			var users = _context.Users.Where(u=>u.IsActive).OrderByDescending(u => u.InsertDate).ToList();
			return users;
		}

		
	}
}
