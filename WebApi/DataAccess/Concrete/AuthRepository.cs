using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers.Hashing;
using WebApi.DataAccess.Abstract;
using WebApi.Entities.Concrete;
using WebApi.Entities.Concrete.Dtos;
using WebApi.DataAccess.Context;

namespace WebApi.DataAccess.Concrete
{
	public class AuthRepository : IAuthRepository
	{
		private readonly DatabaseContext _context;
		private readonly IMapper _mapper;
		private readonly PasswordHasher _passwordHasher;
		public AuthRepository(DatabaseContext context,IMapper mapper, PasswordHasher passwordHasher)
		{
			_context = context;
			_mapper = mapper;
			_passwordHasher = passwordHasher;
		}

		public async Task<User?> LoginUserAsync(LoginDto loginUser, CancellationToken ct = default)
		{
			try
			{
				// Kullanıcıyı sadece kullanıcı adına göre veritabanından çek.
				var user = await _context.Users
					.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
					.FirstOrDefaultAsync(u => u.Username == loginUser.Username, ct);

				if (user is null)
				{
					return null;
				}

				// Kullanıcı bulunduysa, düz metin parolayı ve hash'lenmiş parolayı doğrula.
				// PasswordHasher.VerifyPassword metodu, bcrypt kullanarak karşılaştırmayı yapar.
				bool isPasswordValid = _passwordHasher.VerifyPassword(loginUser.Password, user.Password);

				// Parola geçerliyse kullanıcıyı döndür, aksi halde null döndür.
				return isPasswordValid ? user : null;
			}
			catch (Exception ex)
			{
				// Hata oluştuğunda bu blok çalışacak. Loglama yapmanız önerilir.
				// Örneğin: Log.Error(ex, "Login işlemi sırasında bir hata oluştu.");
				// Şimdilik null döndürelim, böylece 500 hatası yerine 401 döndürür.
				return null;
			}
		}


		public async Task RegisterUserAsync(RegisterDto dto, CancellationToken ct = default)
		{
			var user = _mapper.Map<User>(dto);
			// bcrypt ile parolanın hash'lenmesi
			user.Password = _passwordHasher.HashPassword(user.Password);
			user.InsertDate = DateTime.Now;
			user.IsActive = true;

			await _context.Users.AddAsync(user, ct);
			await _context.SaveChangesAsync(ct);
		}

		public Task<User?> GetUserByIdAsync(int userId, CancellationToken ct = default)
			=> _context.Users
				.Include(ur => ur.UserRoles).ThenInclude(u => u.Role)
				.FirstOrDefaultAsync(u => u.UserId == userId, ct);

		public async Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken ct = default)
		{
			return await _context.Users.AnyAsync(u => u.Username == username || u.Email == email, ct);
		}
	}

}
