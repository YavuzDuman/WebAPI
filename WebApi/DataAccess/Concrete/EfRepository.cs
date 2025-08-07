using WebApi.DataAccess.Abstract;
using WebApi.DataAccess.Context;
using WebApi.Entities.Abstract;

namespace WebApi.DataAccess.Concrete
{
	public class EfRepository<T> : IRepository<T> where T : class, IEntity, new()
	{
		protected readonly DatabaseContext _context;

		public EfRepository(DatabaseContext context)
		{
			_context = context;
		}

		public void Add(T entity)
		{
			_context.Set<T>().Add(entity);
			_context.SaveChanges();
		}

		public void Delete(int id)
		{
			var entity = _context.Set<T>().Find(id);
			if (entity == null) return;
			_context.Set<T>().Remove(entity);
			_context.SaveChanges();
		}

		public List<T> GetAll()
		{
			return _context.Set<T>().ToList();
		}

		public T GetById(int id)
		{
			return _context.Set<T>().Find(id);
		}

		public void Update(int id, T entity)
		{
			var existing = _context.Set<T>().Find(id);
			if (existing == null) return;

			_context.Entry(existing).CurrentValues.SetValues(entity);
			_context.SaveChanges();
		}
	}
}
