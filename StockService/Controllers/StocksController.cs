﻿using Microsoft.AspNetCore.Mvc;
using StockService.Business.Abstract;
using StockService.Entities.Concrete;

namespace StockService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StocksController : ControllerBase
	{
		private readonly IStockManager _stockManager;

		public StocksController(IStockManager stockManager)
		{
			_stockManager = stockManager;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllStocks()
		{
			var stocks = await _stockManager.GetAllStocksAsync();
			return Ok(stocks);
		}

		[HttpGet("{symbol}")]
		public async Task<IActionResult> GetStockBySymbol(string symbol)
		{
			var stock = await _stockManager.GetStockBySymbolAsync(symbol);
			if (stock == null)
			{
				return NotFound($"Stock with symbol {symbol} not found.");
			}
			return Ok(stock);
		}

		[HttpPost("update-from-api")]
		public async Task<IActionResult> UpdateStocksFromApi()
		{
			await _stockManager.UpdateStocksFromExternalApiAsync();
			return Ok("Stocks updated successfully from external API.");
		}
	}
}