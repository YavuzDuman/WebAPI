using WebApi.Entities;
using WebApi.Entities.Dtos;

namespace WebApi.DataAccess.Abstract
{
	public interface IAuthDal
	{
		void RegisterUser(RegisterDto user);
		User LoginUser(LoginDto user);
	}
}
