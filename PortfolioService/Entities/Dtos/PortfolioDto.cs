namespace PortfolioService.Entities.Dtos
{
	public class PortfolioDto
	{
		public int Id { get; set; }
		public string StockSymbol { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public decimal PurchasePrice { get; set; }
		public decimal CurrentPrice { get; set; }
		public decimal ProfitOrLoss { get; set; }
		public DateTime PurchaseDate { get; set; }
	}
}
