using WebApi.Entities;
using WebApi.Entities.Dtos;

namespace WebApi.Services.Abstract
{
	public interface IAuthService
	{
		void RegisterUser(RegisterDto user);
		User LoginUser(LoginDto user);
	}
}
