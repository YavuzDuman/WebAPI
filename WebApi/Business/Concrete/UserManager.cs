using WebApi.Business.Abstract;
using WebApi.DataAccess.Abstract;
using WebApi.Entities;
using WebApi.Entities.Dtos;

namespace WebApi.Business.Concrete
{
	public class UserManager : IUserManager
	{
		private readonly IUserDal _userDal;

		public UserManager(IUserDal userService)
		{
			_userDal = userService;
		}

		public List<UserDto> GetAllUsers()
		{
			return _userDal.GetAllUsersWithRoles().ToList();
		}
		public void CreateUser(User user)
		{
			_userDal.CreateUser(user);
		}

		public void DeleteUser(int id)
		{
			_userDal.DeleteUser(id);
		}

		public List<UserDto> GetAllUsersOrderByDate()
		{
			var users = _userDal.GetAllUsersWithRoles().Where(u=> u.IsActive).OrderByDescending(u => u.InsertDate).ToList(); ;
			return users;
		}

		public UserDto GetUserById(int id)
		{
			return _userDal.GetUserByIdWithRoles(id);
		}

		public void SoftDeleteUserById(int id)
		{
			var user = _userDal.GetById(id);
			if (user != null)
			{
				user.IsActive = false;
				_userDal.UpdateUser(id, user);
			}
		}

		public void UpdateUser(int id, User updatedUser)
		{
			_userDal.UpdateUser(id, updatedUser);
		}
	}
}
