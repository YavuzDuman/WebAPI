using Microsoft.EntityFrameworkCore;
using WebApi.DataAccess.Abstract;
using WebApi.DataAccess.Context;
using WebApi.Entities.Abstract;

namespace WebApi.DataAccess.Concrete
{
	public class EfRepository<T> : IRepository<T> where T : class, IEntity, new()
	{
		protected readonly DatabaseContext _context;
		public EfRepository(DatabaseContext context) => _context = context;

		public async Task AddAsync(T entity, CancellationToken ct = default)
		{
			await _context.Set<T>().AddAsync(entity, ct);
			await _context.SaveChangesAsync(ct);
		}

		public async Task DeleteAsync(int id, CancellationToken ct = default)
		{
			var entity = await _context.Set<T>().FindAsync([id], ct);
			if (entity is null) return;
			_context.Set<T>().Remove(entity);
			await _context.SaveChangesAsync(ct);
		}

		public Task<List<T>> GetAllAsync(CancellationToken ct = default)
			=> _context.Set<T>().ToListAsync(ct);

		public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
			=> await _context.Set<T>().FindAsync([id], ct);

		public async Task UpdateAsync(int id, T entity, CancellationToken ct = default)
		{
			var existing = await _context.Set<T>().FindAsync([id], ct);
			if (existing is null) return;
			_context.Entry(existing).CurrentValues.SetValues(entity);
			await _context.SaveChangesAsync(ct);
		}
	}

}
