using AutoMapper;
using WebApi.Business.Abstract;
using WebApi.DataAccess.Abstract;
using WebApi.Entities.Concrete;
using WebApi.Entities.Concrete.Dtos;
using WebApi.Helpers.Hashing;

namespace WebApi.Business.Concrete
{
	public class UserManager : IUserManager
	{
		private readonly IUserRepository _user;
		private readonly IMapper _mapper;	

		public UserManager(IUserRepository userService, IMapper mapper)
		{
			_user = userService;
			_mapper = mapper;
		}

		public List<UserDto> GetAllUsers()
		{
			var users =  _user.GetAllWithRoles().Where(u=>u.IsActive).ToList();
			return _mapper.Map<List<UserDto>>(users);
		}
		public void CreateUser(User user)
		{
			user.Password = PasswordHasher.HashPassword(user.Password);
			user.InsertDate = DateTime.Now;
			user.IsActive = true;
			_user.Add(user);
		}

		public void DeleteUser(int id)
		{
			_user.Delete(id);
		}

		public List<UserDto> GetAllUsersOrderByDate()
		{
			var users = _user.GetAllWithRoles().Where(u=> u.IsActive).OrderByDescending(u => u.InsertDate).ToList(); ;
			return _mapper.Map<List<UserDto>>(users);
		}

		public UserDto GetUserById(int id)
		{
			var user = _user.GetByIdWithRoles(id);
			return user != null ? _mapper.Map<UserDto>(user) : null;
		}

		public void SoftDeleteUserById(int id)
		{
			var user = _user.GetById(id);
			if (user != null)
			{
				user.IsActive = false;
				_user.Update(id, user);
			}
		}

		public void UpdateUser(int id, User updatedUser)
		{
			_user.Update(id, updatedUser);
		}
	}
}
