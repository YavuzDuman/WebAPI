using WebApi.Entities.Abstract;

namespace WebApi.DataAccess.Abstract
{
	public interface IRepository<T> where T : class, IEntity, new()
	{
		List<T> GetAll();
		T GetById(int id);
		void Add(T entity);
		void Update(int id,T entity);
		void Delete(int id);
	}
}
