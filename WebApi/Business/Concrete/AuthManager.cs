using WebApi.Business.Abstract;
using WebApi.DataAccess.Abstract;
using WebApi.Entities.Concrete.Dtos;
using WebApi.Entities.Concrete;
using AutoMapper;
using WebApi.Helpers.Hashing;

namespace WebApi.Business.Concrete
{
	public class AuthManager : IAuthManager
	{
		private readonly IAuthRepository _repo;
		private readonly PasswordHasher _passwordHasher; // Eklendi
		private readonly IMapper _mapper; // Eklendi

		public AuthManager(IAuthRepository repo, PasswordHasher passwordHasher, IMapper mapper) // Bağımlılıklar eklendi
		{
			_repo = repo;
			_passwordHasher = passwordHasher;
			_mapper = mapper;
		}

		public Task<User?> LoginUserAsync(LoginDto loginUser, CancellationToken ct = default)
			=> _repo.LoginUserAsync(loginUser, ct);

		public async Task<bool> RegisterUserAsync(RegisterDto dto, CancellationToken ct = default)
		{
			var exists = await _repo.ExistsByUsernameOrEmailAsync(dto.Username, dto.Email, ct);
			if (exists) return false;

			await _repo.RegisterUserAsync(dto, ct);
			return true;
		}

		public Task<User?> GetUserByIdAsync(int userId, CancellationToken ct = default)
			=> _repo.GetUserByIdAsync(userId, ct);

	}
}
