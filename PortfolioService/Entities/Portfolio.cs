using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioService.Entities
{
	[Table("Portfolios")]
	public class Portfolio
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public string StockSymbol { get; set; } = string.Empty;

		public int Quantity { get; set; }

		[Column(TypeName = "decimal(18, 2)")]
		public decimal PurchasePrice { get; set; }

		public DateTime PurchaseDate { get; set; }
	}
}
