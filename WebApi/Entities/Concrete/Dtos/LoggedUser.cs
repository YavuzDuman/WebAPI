using WebApi.Entities.Abstract;

namespace WebApi.Entities.Concrete.Dtos
{
	public class LoggedUser : IDto
	{
		public int UserId { get; set; }
		public string Username { get; set; }
		public DateTime LoginTime { get; set; }
		public DateTime ExpireTime { get; set; }
	}
}
