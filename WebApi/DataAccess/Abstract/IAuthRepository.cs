using WebApi.Entities.Concrete;
using WebApi.Entities.Concrete.Dtos;

namespace WebApi.DataAccess.Abstract
{
	public interface IAuthRepository
	{
		Task<User?> LoginUserAsync(LoginDto loginUser, CancellationToken ct = default);
		Task RegisterUserAsync(RegisterDto dto, CancellationToken ct = default);
		Task<User?> GetUserByIdAsync(int userId, CancellationToken ct = default);

		Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken ct = default);
	}
}
