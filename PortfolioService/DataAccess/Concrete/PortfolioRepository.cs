using Dapper;
using PortfolioService.DataAccess.Abstract;
using PortfolioService.Entities;
using System.Data;

namespace PortfolioService.DataAccess.Concrete
{
	public class PortfolioRepository : IPortfolioRepository
	{
		private readonly IDbConnection _dbConnection;

		public PortfolioRepository(IDbConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public async Task AddAsync(Portfolio portfolio)
		{
			// SQL sorgusu, nesnedeki PurchaseDate özelliğiyle eşleşmesi için düzeltildi
			var sql = "INSERT INTO Portfolios (UserId, StockSymbol, Quantity, PurchasePrice, PurchaseDate) VALUES (@UserId, @StockSymbol, @Quantity, @PurchasePrice, @PurchaseDate)";
			await _dbConnection.ExecuteAsync(sql, portfolio);
		}

		public async Task DeleteAsync(int porfolioId)
		{
			var sql = "DELETE FROM Portfolios WHERE Id = @Id";
			await _dbConnection.ExecuteAsync(sql, new { Id = porfolioId });
		}

		public async Task<IEnumerable<Portfolio>> GetPortfoliosByUserIdAsync(int userId)
		{
			var sql = "SELECT * FROM Portfolios WHERE UserId = @UserId";
			return await _dbConnection.QueryAsync<Portfolio>(sql, new { UserId = userId });
		}

		public async Task<Portfolio?> GetByIdAsync(int portfolioId)
		{
			var sql = "Select * from Portfolios where Id = @Id";
			return await _dbConnection.QueryFirstOrDefaultAsync<Portfolio>(sql, new { Id = portfolioId });
		}
	}
}
