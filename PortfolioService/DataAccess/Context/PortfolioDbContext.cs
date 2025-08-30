using Microsoft.EntityFrameworkCore;
using PortfolioService.Entities;
namespace PortfolioService.DataAccess.Context
{
	public class PortfolioDbContext : DbContext
	{
		public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options)
		{
		}

		public DbSet<Portfolio> Portfolios { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Veritabanı tablo adını belirtin
			modelBuilder.Entity<Portfolio>().ToTable("Portfolios");

			// Id alanını birincil anahtar olarak belirtin
			modelBuilder.Entity<Portfolio>().HasKey(p => p.Id);

			// Gerekli diğer yapılandırmaları yapabilirsiniz
			// Örneğin:
			// modelBuilder.Entity<Portfolio>().Property(p => p.StockSymbol).IsRequired().HasMaxLength(10);
			// modelBuilder.Entity<Portfolio>().Property(p => p.PurchasePrice).HasColumnType("decimal(18, 2)");
		}
	}
}
