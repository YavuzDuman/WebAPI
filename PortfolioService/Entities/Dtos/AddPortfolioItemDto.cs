namespace PortfolioService.Entities.Dtos
{
	public class AddPortfolioItemDto
	{
		public int UserId { get; set; }
		public string StockSymbol { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public decimal PurchasePrice { get; set; }
	}
}
