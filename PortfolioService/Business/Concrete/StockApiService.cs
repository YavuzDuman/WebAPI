using PortfolioService.Business.Abstract;
using PortfolioService.Entities.Dtos;

namespace PortfolioService.Business.Concrete
{
	public class StockApiService : IStockApiService
	{
		private readonly HttpClient _httpClient;
		private readonly string _stockServiceUrl;

		// Constructor ile HttpClient ve IConfiguration bağımlılıkları enjekte edilir.
		public StockApiService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_stockServiceUrl = configuration["StockServiceUrl"] ?? string.Empty;
		}

		public async Task<decimal> GetStockPriceAsync(string symbol)
		{
			// StockService'in fiyat çekme endpoint'ine HTTP GET isteği gönderir.
			// URL şuna benzer olmalı: http://localhost:5000/api/stocks/{symbol}
			var requestUrl = $"{_stockServiceUrl}/api/stocks/{symbol}";

			try
			{
				var response = await _httpClient.GetAsync(requestUrl);
				response.EnsureSuccessStatusCode(); // 2xx başarılı durum kodlarını kontrol et.

				// Yanıttan doğrudan fiyat değerini (decimal) veya bir DTO nesnesini al.
				// Bizim StockService projesindeki GetStockBySymbol endpoint'i doğrudan Stock objesi döndürüyor.
				var stock = await response.Content.ReadFromJsonAsync<StockApiDto>();

				// Eğer veri gelirse, fiyatı döndür. Yoksa 0 döndür.
				return stock?.CurrentPrice ?? 0m;
			}
			catch (HttpRequestException ex)
			{
				// İstek başarısız olursa, hatayı logla ve 0 döndür.
				Console.WriteLine($"Error fetching stock price for {symbol}: {ex.Message}");
				return 0m;
			}
		}
	}
}
