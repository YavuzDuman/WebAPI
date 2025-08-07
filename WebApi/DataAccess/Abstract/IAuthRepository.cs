using WebApi.Entities.Concrete;
using WebApi.Entities.Concrete.Dtos;

namespace WebApi.DataAccess.Abstract
{
	public interface IAuthRepository
	{
		void RegisterUser(RegisterDto user);
		User LoginUser(LoginDto user);
		Task<User> GetUserByIdAsync(int userId);
	}
}
