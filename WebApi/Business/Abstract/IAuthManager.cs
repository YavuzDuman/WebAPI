using WebApi.Entities.Concrete;
using WebApi.Entities.Concrete.Dtos;

namespace WebApi.Business.Abstract
{
	public interface IAuthManager
	{
		Task<User?> LoginUserAsync(LoginDto loginUser, CancellationToken ct = default);
		Task<bool> RegisterUserAsync(RegisterDto dto, CancellationToken ct = default);
		Task<User?> GetUserByIdAsync(int userId, CancellationToken ct = default);

	}
}
