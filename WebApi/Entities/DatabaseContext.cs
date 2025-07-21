using Microsoft.EntityFrameworkCore;
using WebApi.Entities.Dtos;

namespace WebApi.Entities
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options)
			: base(options)
		{
		}
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }

		public DbSet<UserDto> UsersWithRolesDto { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserDto>().HasNoKey(); 
			base.OnModelCreating(modelBuilder);
		}
	}
}
