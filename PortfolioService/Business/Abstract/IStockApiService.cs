namespace PortfolioService.Business.Abstract
{
	public interface IStockApiService
	{
		// Bir hisse senedinin anlık fiyatını StockService'ten asenkron olarak çeker.
		Task<decimal> GetStockPriceAsync(string symbol);
	}
}
