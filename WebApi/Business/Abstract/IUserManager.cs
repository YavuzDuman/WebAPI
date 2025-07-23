using WebApi.Entities;
using WebApi.Entities.Dtos;

namespace WebApi.Business.Abstract
{
	public interface IUserManager
	{
		List<UserDto> GetAllUsers();
		UserDto GetUserById(int id);
		void CreateUser(User user);
		void UpdateUser(int id, User updatedUser);
		void DeleteUser(int id);
		void SoftDeleteUserById(int id);
		List<UserDto> GetAllUsersOrderByDate();

	}
}
