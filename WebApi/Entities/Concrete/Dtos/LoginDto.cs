using WebApi.Entities.Abstract;

namespace WebApi.Entities.Concrete.Dtos
{
	public class LoginDto : IDto
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
