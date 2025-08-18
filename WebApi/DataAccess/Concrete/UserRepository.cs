using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers.Hashing;
using WebApi.DataAccess.Abstract;
using WebApi.Entities.Concrete;
using WebApi.DataAccess.Context;

namespace WebApi.DataAccess.Concrete
{
	public class UserRepository : EfRepository<User>, IUserRepository
	{
		private readonly DatabaseContext _db;
		public UserRepository(DatabaseContext context) : base(context) => _db = context;

		public Task<List<User>> GetAllWithRolesAsync(CancellationToken ct = default)
			=> _db.Users
				  .Include(u => u.UserRoles)
				  .ThenInclude(ur => ur.Role)
				  .ToListAsync(ct);

		public Task<User?> GetByIdWithRolesAsync(int id, CancellationToken ct = default)
			=> _db.Users
				  .Include(u => u.UserRoles)
				  .ThenInclude(ur => ur.Role)
				  .FirstOrDefaultAsync(u => u.UserId == id, ct);
	}
}
