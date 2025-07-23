namespace WebApi.Entities
{
	public class LoggedUser
	{
		public int UserId { get; set; }
		public string Username { get; set; }
		public DateTime LoginTime { get; set; }
		public DateTime ExpireTime { get; set; }
	}
}
