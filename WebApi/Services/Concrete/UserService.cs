using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Entities.Dtos;
using WebApi.Helpers.Hashing;
using WebApi.Services.Abstract;

namespace WebApi.Services.Concrete
{
	public class UserService : IUserService
	{
		private readonly DatabaseContext _context;
		private readonly IMapper _mapper;

		public UserService(DatabaseContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public List<UserDto> GetAllUsers()
		{
			var users = _context.Users
				.Include<User, Role>(u => u.Role)
				.Where(u => u.IsActive == true).ToList();
			return _mapper.Map<List<UserDto>>(users);
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

		public UserDto GetUserById(int id)
		{
			var user = _context.Users
				.Include<User, Role>(u => u.Role)
				.FirstOrDefault(u => u.UserId == id && u.IsActive);
			return _mapper.Map<UserDto>(user);
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

		public List<UserDto> GetAllUsersOrderByDate()
		{
			var users = _context.Users
				.Include<User, Role>(u => u.Role)
				.Where(u=>u.IsActive).OrderByDescending(u => u.InsertDate).ToList();
			return _mapper.Map < List < UserDto>>(users);
		}

		
	}
}
