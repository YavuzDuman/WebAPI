using WebApi.Entities.Abstract;

namespace WebApi.Entities.Concrete.Dtos
{
	public class RegisterDto : IDto
	{
		public string Name { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public bool IsActive { get; set; }
		public string Password { get; set; }
		public DateTime? InsertDate { get; set; }
	}
}
