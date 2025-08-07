using WebApi.Entities.Concrete;

namespace WebApi.DataAccess.Abstract
{
	public interface IUserRepository : IRepository<User>
	{
		User GetByIdWithRoles(int id);
		List<User> GetAllWithRoles();
	}
}
