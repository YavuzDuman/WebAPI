using PortfolioService.Entities.Dtos;

namespace PortfolioService.Business.Abstract
{
	public interface IPortfolioManager
	{
		Task<List<PortfolioDto>> GetUserPortfolioAsync(int userId);
		Task AddStockToPortfolioAsync(int userId, string stockSymbol, int quantity, decimal purchasePrice);
		Task<bool> RemoveStockFromPortfolioAsync(int portfolioId);
	}
}
