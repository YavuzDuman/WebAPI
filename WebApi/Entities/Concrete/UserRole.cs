﻿using WebApi.Entities.Abstract;

namespace WebApi.Entities.Concrete
{
	public class UserRole : IEntity
	{
		public int UserId { get; set; }
		public int RoleId { get; set; }

		public User User { get; set; }
		public Role Role { get; set; }
	}
}
