using WebApi.Entities;
using WebApi.Entities.Dtos;

namespace WebApi.DataAccess.Abstract
{
	public interface IUserDal
	{
		List<UserDto> GetAllUsersWithRoles();
		UserDto GetUserByIdWithRoles(int id);
		void CreateUser(User user);
		void UpdateUser(int id, User updatedUser);
		void DeleteUser(int id);
		User GetById(int id);
		List<User> GetAllUsers();
		//void SoftDeleteUserById(int id);
		//List<UserDto> GetAllUsersOrderByDate();

	}
}
