using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Entities.Dtos;
using WebApi.Helpers.Hashing;
using WebApi.DataAccess.Abstract;

namespace WebApi.DataAccess.Concrete
{
	public class UserDal : IUserDal
	{
		private readonly DatabaseContext _context;
		private readonly IMapper _mapper;

		public UserDal(DatabaseContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public List<UserDto> GetAllUsersWithRoles()
		{
			var users = _context.Users
				.Include(u => u.UserRoles)
				.ThenInclude(ur => ur.Role)
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
		public User GetById(int id)
		{
			var user = _context.Users.FirstOrDefault(u => u.UserId == id);
			return user ?? throw new KeyNotFoundException($"User with ID {id} not found.");
		}

		public UserDto GetUserByIdWithRoles(int id)
		{
			var user = _context.Users
				.Include(u => u.UserRoles)
				.ThenInclude(ur => ur.Role)
				.FirstOrDefault(u => u.UserId == id );
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

		public List<User> GetAllUsers()
		{
			var users = _context.Users.ToList();
			return users;
		}


		//public void SoftDeleteUserById(int id)
		//{
		//	var user = _context.Users.FirstOrDefault(u => u.UserId == id);
		//	if (user == null) return;
		//	user.IsActive = false;
		//	_context.SaveChanges();
		//}

		//public List<UserDto> GetAllUsersOrderByDate()
		//{
		//	var users = _context.Users
		//		.Include(u => u.UserRoles)
		//		.Where(u=>u.IsActive).OrderByDescending(u => u.InsertDate).ToList();
		//	return _mapper.Map < List < UserDto>>(users);
		//}


	}
}
