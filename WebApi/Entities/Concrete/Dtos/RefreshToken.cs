using WebApi.Entities.Abstract;

namespace WebApi.Entities.Concrete.Dtos
{
	public class RefreshToken : IDto
	{
		public string Token { get; set; }
		public int UserId { get; set; }
		public DateTime ExpireTime { get; set; }
	}
}
