namespace WebApi.Entities.Dtos
{
	public class UserDto
	{
		public int UserId { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string RoleName { get; set; }
		public DateTime InsertDate { get; set; }
		public bool IsActive { get; set; }
	}
}
