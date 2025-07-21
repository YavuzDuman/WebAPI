using WebApi.Entities;
using WebApi.Entities.Dtos;

namespace WebApi.Services.Abstract
{
	public interface IUserService
	{
		List<User> GetAllUsers();
		User GetUserById(int id);
		void CreateUser(User user);
		void UpdateUser(int id, User updatedUser);
		void DeleteUser(int id);
		void SoftDeleteUserById(int id);
		List<User> GetAllUsersOrderByDate();

	}
}
