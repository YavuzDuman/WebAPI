using WebApi.Entities.Abstract;

namespace WebApi.Entities.Concrete
{
	public class Role : IEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public ICollection<UserRole> UserRoles { get; set; }
	}
}
