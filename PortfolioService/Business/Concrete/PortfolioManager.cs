using PortfolioService.Business.Abstract;
using PortfolioService.DataAccess.Abstract;
using PortfolioService.Entities;
using PortfolioService.Entities.Dtos;

namespace PortfolioService.Business.Concrete
{
	public class PortfolioManager : IPortfolioManager
	{
		private readonly IPortfolioRepository _portfolioRepository;
		private readonly IStockApiService _stockApiService;

		public PortfolioManager(IPortfolioRepository portfolioRepository, IStockApiService stockApiService)
		{
			_portfolioRepository = portfolioRepository;
			_stockApiService = stockApiService;
		}

		public async Task<List<PortfolioDto>> GetUserPortfolioAsync(int userId)
		{
			var userPortfolios = await _portfolioRepository.GetPortfoliosByUserIdAsync(userId);
			var portfolioDtos = new List<PortfolioDto>();

			foreach (var portfolioItem in userPortfolios)
			{
				// StockService'ten anlık fiyatı al
				var currentPrice = await _stockApiService.GetStockPriceAsync(portfolioItem.StockSymbol);

				var portfolioDto = new PortfolioDto
				{
					Id = portfolioItem.Id,
					StockSymbol = portfolioItem.StockSymbol,
					Quantity = portfolioItem.Quantity,
					PurchasePrice = portfolioItem.PurchasePrice,
					CurrentPrice = currentPrice,
					PurchaseDate = portfolioItem.PurchaseDate,
					// Kar/Zarar hesaplaması
					ProfitOrLoss = (currentPrice - portfolioItem.PurchasePrice) * portfolioItem.Quantity
				};
				portfolioDtos.Add(portfolioDto);
			}
			return portfolioDtos;
		}

		public async Task AddStockToPortfolioAsync(int userId, string stockSymbol, int quantity, decimal purchasePrice)
		{
			var newPortfolio = new Portfolio
			{
				UserId = userId,
				StockSymbol = stockSymbol,
				Quantity = quantity,
				PurchasePrice = purchasePrice,
				PurchaseDate = DateTime.Now
			};
			await _portfolioRepository.AddAsync(newPortfolio);
		}

		public async Task<bool> RemoveStockFromPortfolioAsync(int portfolioId)
		{
			var portfolioToRemove = await _portfolioRepository.GetByIdAsync(portfolioId);
			if (portfolioToRemove == null)
			{
				return false;
			}

			await _portfolioRepository.DeleteAsync(portfolioToRemove.Id);
			return true;
		}
	}
}
