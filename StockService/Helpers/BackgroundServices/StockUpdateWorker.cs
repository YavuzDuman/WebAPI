using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration; // IConfiguration için eklendi
using StockService.Business.Abstract;
using Microsoft.AspNetCore.SignalR;
using StockService.Hubs;

namespace StockService.BackgroundServices
{
	public class StockUpdateWorker : BackgroundService
	{
		private readonly ILogger<StockUpdateWorker> _logger;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly IHubContext<StockHub> _hubContext;
		private readonly int _updateIntervalInMinutes; // Yeni alan

		public StockUpdateWorker(
			ILogger<StockUpdateWorker> logger,
			IServiceScopeFactory scopeFactory,
			IHubContext<StockHub> hubContext,
			IConfiguration configuration) // IConfiguration eklendi
		{
			_logger = logger;
			_scopeFactory = scopeFactory;
			_hubContext = hubContext;
			// appsettings.json dosyasından güncelleme aralığını oku
			_updateIntervalInMinutes = configuration.GetValue<int>("WorkerSettings:StockUpdateIntervalInMinutes");
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Stock Update Worker başlatıldı. Güncelleme aralığı: {UpdateInterval} dakika.", _updateIntervalInMinutes);

			int retryCount = 0;
			int maxRetries = 3;

			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation("Hisse senedi verileri güncelleniyor...");

				try
				{
					using (var scope = _scopeFactory.CreateScope())
					{
						var stockManager = scope.ServiceProvider.GetRequiredService<IStockManager>();

						// Adım 1: Veritabanını güncelle
						await stockManager.UpdateStocksFromExternalApiAsync();

						_logger.LogInformation("Hisse senedi verileri başarıyla veritabanında güncellendi.");

						// Adım 2: Redis'e veriyi kaydetmek için GetAllStocksAsync'i çağır
						await stockManager.GetAllStocksAsync();

						_logger.LogInformation("Hisse senedi verileri Redis cache'ine yazıldı.");

						// Adım 3: Güncelleme tamamlandığında SignalR üzerinden sinyal gönder
						await _hubContext.Clients.All.SendAsync("ReceiveStockUpdate", "Hisse senedi verileri güncellendi.");
					}

					retryCount = 0; // Başarılı olursa deneme sayacını sıfırla
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Hisse senedi verilerini güncellerken bir hata oluştu.");
					if (retryCount < maxRetries)
					{
						retryCount++;
						_logger.LogWarning("Yeniden deneme ({RetryCount}/{MaxRetries}).", retryCount, maxRetries);
						await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // 30 saniye sonra tekrar dene
						continue; // Döngü başına dön
					}
					_logger.LogError("Yeniden deneme limitine ulaşıldı. Sonraki planlı çalıştırmaya kadar bekleniyor.");
				}

				await Task.Delay(TimeSpan.FromMinutes(_updateIntervalInMinutes), stoppingToken);
			}

			_logger.LogInformation("Stock Update Worker sonlandırıldı.");
		}
	}
}
