using Microsoft.AspNetCore.Mvc;
using PortfolioService.Business.Abstract;
using PortfolioService.Entities.Dtos;

namespace PortfolioService.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PortfolioController : ControllerBase
	{
		private readonly IPortfolioManager _portfolioManager;

		public PortfolioController(IPortfolioManager portfolioManager)
		{
			_portfolioManager = portfolioManager;
		}

		[HttpGet("{userId}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PortfolioDto>))]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetUserPortfolio(int userId)
		{
			var portfolio = await _portfolioManager.GetUserPortfolioAsync(userId);
			if (portfolio == null || !portfolio.Any())
			{
				return NotFound("Kullanıcı portföyü bulunamadı.");
			}
			return Ok(portfolio);
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> AddStockToPortfolio([FromBody] AddPortfolioItemDto dto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			await _portfolioManager.AddStockToPortfolioAsync(dto.UserId, dto.StockSymbol, dto.Quantity, dto.PurchasePrice);
			return StatusCode(201, "Hisse senedi portföye başarıyla eklendi.");
		}

		[HttpDelete("{portfolioId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> RemoveStockFromPortfolio(int portfolioId)
		{
			var result = await _portfolioManager.RemoveStockFromPortfolioAsync(portfolioId);
			if (!result)
			{
				return NotFound("Silinecek portföy kalemi bulunamadı.");
			}
			return Ok("Hisse senedi portföyden başarıyla kaldırıldı.");
		}
	}

	// Yeni kayıt için kullanılacak DTO
	
}
