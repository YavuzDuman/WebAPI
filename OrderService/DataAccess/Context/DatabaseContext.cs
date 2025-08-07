using Microsoft.EntityFrameworkCore;
using OrderService.Entities;

namespace OrderService.DataAccess.Context
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

		public DbSet<Order> Orders { get; set; }
	}
}
