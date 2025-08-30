using PortfolioService.Entities;

namespace PortfolioService.DataAccess.Abstract
{
	public interface IPortfolioRepository
	{
		Task<IEnumerable<Portfolio>> GetPortfoliosByUserIdAsync(int userId);
		Task AddAsync(Portfolio portfolio);
		Task DeleteAsync(int porfolioId);
		Task<Portfolio?> GetByIdAsync(int portfolioId);
	}
}
