namespace WebApi.Entities
{
	public class User
	{
		public int UserId { get; set; }
		public string Name { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public bool IsActive { get; set; }
		public string Password { get; set; }
		public DateTime InsertDate { get; set; }
		public int RoleId { get; set; }
		public Role Role { get; set; }
	}
}
